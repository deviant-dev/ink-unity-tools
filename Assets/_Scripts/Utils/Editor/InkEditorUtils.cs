using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;

namespace Deviant.Utils {
	public static class InkEditorUtils {
		public static readonly Regex TagRegex = new Regex(@"#(?<tag>[0-9]+)(\s|$)");
		public static readonly Regex KnotRegex = new Regex(@"^\s*=[=]+\s*(?<knot>\w+)");

		[MenuItem("Assets/Tag Ink Lines", false, 70)]
		public static void TagLines() {
			string path = AssetDatabase.GetAssetPath(Selection.activeObject);

			if (!File.Exists(path)) {
				Debug.LogErrorFormat(Selection.activeObject, Selection.activeObject + " isn't an ink file.");
				return;
			}

			string[] lines = File.ReadAllLines(path);
			Dictionary<int, HashSet<int>> tagIds = GatherTags(lines);
			if (TagUntaggedLines(lines, tagIds)) { SaveLines(lines, path); }
			else { Debug.LogFormat("No tags needed."); }
		}

		[MenuItem("Assets/Retag Ink Lines", false, 71)]
		public static void RetagLines() {
			string path = AssetDatabase.GetAssetPath(Selection.activeObject);

			if (!File.Exists(path)) {
				Debug.LogErrorFormat(Selection.activeObject, Selection.activeObject + " isn't an ink file.");
				return;
			}

			string[] lines = File.ReadAllLines(path);
			bool changed = RemoveTags(lines);
			changed = TagUntaggedLines(lines) || changed;
			if (changed) { SaveLines(lines, path); }
			else { Debug.LogFormat("No tags needed."); }
		}

		private static void SaveLines(string[] lines, string path) {
			if (Provider.isActive) { Provider.Checkout(Selection.activeObject, CheckoutMode.Both).Wait(); }
			File.WriteAllLines(path, lines);
			AssetDatabase.ImportAsset(path);
		}

		private static bool RemoveTags(string[] lines) {
			bool changed = false;

			for (int i = 0; i < lines.Length; i++) {
				string line = lines[i].ReplaceRegex(@"\s*#(?<tag>[0-9]+)(\s*|$)", "");
				if (lines[i] == line) { continue; }
				lines[i] = line;
				changed = true;
			}

			return changed;
		}

		private static bool TagUntaggedLines(string[] lines, Dictionary<int, HashSet<int>> tagSets = null) {
			tagSets = tagSets ?? new Dictionary<int, HashSet<int>>();

			int currentKnot = 0;
			HashSet<int> existingTags = GetTagSet(currentKnot, tagSets);

			int currentId = 1;
			bool changed = false;

			for (int i = 0; i < lines.Length; i++) {
				string line = lines[i];
				
				if (line.ContainsRegex(@"^\s*==")) {
					currentKnot++;
					currentId = 1;
					existingTags = GetTagSet(currentKnot, tagSets);
					continue;
				}

				if (!CanTag(line)) { continue; }
				lines[i] = line + " #" + GetNextId(ref currentId, existingTags).ToString("00");
				changed = true;
			}

			return changed;
		}

		private static HashSet<int> GetTagSet(int knotNumber, Dictionary<int, HashSet<int>> tagSets) {
			if (tagSets.ContainsKey(knotNumber)) { return tagSets[knotNumber]; }
			return tagSets[knotNumber] = new HashSet<int>();
		}

		private static bool CanTag(string line) {
			string originalLine = line;

			// If we already have a tag, then we can't tag.
			if (line.Contains('#') && TagRegex.IsMatch(line)) { return false; }

			// Remove any '-> Ref' bits.
			line = line.ReplaceRegex(@"\-\>\s*[a-zA-Z0-9_.]+", "");

			// Remove '- else:'
			line = line.ReplaceRegex(@"^\s*-\s*else:", "", RegexOptions.IgnoreCase);

			// Skip '<blockquote>' passages used for the web stuff.
			if (line.ContainsRegex(@"^\s*<blockquote>", RegexOptions.IgnoreCase)) { return false; }

			// Remove labels
			line = line.ReplaceRegex(@"^\s*(\+|\-)\s*(\(\s*[a-zA-Z0-9_.]+\s*\))?", "").Trim();

			// Ignore VAR lines.
			if (line.StartsWith("VAR")) { return false; }

			// Skip 'TODO:'s
			if (line.ContainsRegex(@"^\s*TODO:")) { return false; }

			// Ignore code lines.
			if (line.StartsWith("~")) { return false; }

			// Ignore command lines.
			if (line.StartsWith("/")) { return false; }

			// Ignore knots and stitches.
			if (line.StartsWith("=")) { return false; }

			// Remove in-line code
			line = line.ReplaceRegex(@"{[^}]*}", "");

			// Remove list type
			line = line.ReplaceRegex(@"{[^:]*:", "");

			// Remove list logic (e.g. '- else:')
			line = line.ReplaceRegex(@"^\s*[-]+\s*[a-zA-Z0-9_]+\s*:", "");

			// Remove any remaining trim.
			line = line.Trim();

			// Remove choices since they don't have VO
			line = line.ReplaceRegex(@"(\+|\*)?\s*\[[^\]]*\]", "");

			// Return true if we have text remaining, otherwise false.
			bool result = line.ContainsRegex("[a-zA-Z]");

			if (result) { Debug.LogFormat("This line is missing a tag: {0}\nOriginal line: {1}", line, originalLine); }

			return result;
		}

		private static int GetNextId(ref int currentId, ISet<int> existingTags) {
			while (existingTags.Contains(currentId)) { currentId++; }
			existingTags.Add(currentId);
			return currentId;
		}

		private static Dictionary<int, HashSet<int>> GatherTags(IEnumerable<string> lines) {
			int currentKnot = 0;
			Dictionary<int, HashSet<int>> tagSets = new Dictionary<int, HashSet<int>>();
			HashSet<int> tags = GetTagSet(currentKnot, tagSets);

			foreach (string line in lines) {

				if (line.ContainsRegex(@"^\s*==")) {
					currentKnot++;
					tags = GetTagSet(currentKnot, tagSets);
					continue;
				}

				if (!line.Contains('#')) { continue; }

				MatchCollection matches = TagRegex.Matches(line);

				for (int i = 0; i < matches.Count; i++) {
					Match match = matches[i];
					if (int.TryParse(match.Groups["tag"].Value, out int tagId)) {
						if (tags.Contains(tagId)) { Debug.LogErrorFormat("Duplicate tag found on line {0}: {1}", i, tagId); }
						else { tags.Add(tagId); }
					}
				}
			}

			return tagSets;
		}
	}
}