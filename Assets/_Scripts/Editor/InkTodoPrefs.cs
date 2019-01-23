using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Deviant.Utils;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Deviant.InkTodo {
	public class InkTodoPrefs : ScriptableObject {
		[SerializeField] private List<DefaultAsset> m_InkFileAssets;
		[SerializeField] private List<RegexTodoSearch> m_Searches = new List<RegexTodoSearch>();
		[SerializeField] private bool m_MissingOnly;
		[SerializeField] private Vector2 m_ScrollPos;
		[SerializeField] private bool m_ShowSummaryByFile;

		public void Analyze() {
			if (m_InkFileAssets == null || m_InkFileAssets.Count == 0) {
				Debug.LogWarningFormat(this, "No ink file found.");
				return;
			}

			string rootDirectory = FindCommonDirectory(m_InkFileAssets.Select(AssetDatabase.GetAssetPath).ToArray());

			Queue<InkFileInfo> queue = new Queue<InkFileInfo>(m_InkFileAssets.Select(a => new InkFileInfo(a)));
			List<InkFileInfo> inkFiles = new List<InkFileInfo>();
			
			m_Searches.ForEach(s => s.Reset());

			while (queue.Count > 0) {
				InkFileInfo info = queue.Dequeue();
				inkFiles.Add(info);

				foreach (string includePath in info.Includes) {
					if (inkFiles.Any(f => f.Path.Equals(includePath, StringComparison.OrdinalIgnoreCase))) { continue; }
					m_InkFileAssets.Add(AssetDatabase.LoadAssetAtPath<DefaultAsset>(includePath));
					queue.Enqueue(new InkFileInfo(includePath, rootDirectory));
				}

				foreach (RegexTodoSearch search in m_Searches) { search.Analyze(info); }
			}

			foreach (RegexTodoSearch search in m_Searches) { search.FinishAnalysis(); }
		}

		private static string FindCommonDirectory(ICollection<string> paths) {
			if (paths == null || paths.Count == 0) { return null; }
			if (paths.Count == 1) { return Path.GetDirectoryName(paths.First()); }

			List<string[]> splitPaths = paths.Select(Path.GetDirectoryName).Select(p => p == null ? new string[0] : p.Split('\\', '/')).ToList();

			int min = splitPaths.Min(a => a.Length);
			int i;
			
			for (i = 0; i < min; i++) {
				string reference = splitPaths[0][i];
				if (splitPaths.Any(a => !string.Equals(a[i], reference, StringComparison.OrdinalIgnoreCase))) { break; }
			}

			return string.Join("/", splitPaths[0].Take(i));
		}

		public void OnGUI() {
			m_MissingOnly = EditorGUILayout.Toggle("Show Todos Only", m_MissingOnly);
			m_ShowSummaryByFile = EditorGUILayout.Toggle("Show Summary By File", m_ShowSummaryByFile);

			using (new GUILayout.HorizontalScope()) {
				if (GUILayout.Button("Analyze")) { Analyze(); }
				if (GUILayout.Button("Export")) { Export(); }
			}

			m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos);

			RegexTodoSearch deleteMe = null;
			RegexTodoSearch moveUp = null;
			RegexTodoSearch moveDown = null;

			for (int i = 0; i < m_Searches.Count; i++) {
				RegexTodoSearch search = m_Searches[i];
				search.OnGUI(m_MissingOnly, i == 0, i == m_Searches.Count - 1, m_ShowSummaryByFile);
				if (search.DeleteMe) { deleteMe = search; }
				if (search.MoveUp) { moveUp = search; }
				if (search.MoveDown) { moveDown = search; }
			}

			if (deleteMe != null) { m_Searches.Remove(deleteMe); }

			else if (moveUp != null) {
				m_Searches.MoveUp(moveUp);
				moveUp.MoveUp = false;
			}
			else if (moveDown != null) {
				m_Searches.MoveDown(moveDown);
				moveDown.MoveDown = false;
			}

			using (new GUILayout.HorizontalScope()) {
				GUILayout.FlexibleSpace();
				if (GUILayout.Button("+")) { m_Searches.Add(new RegexTodoSearch()); }
			}

			EditorGUILayout.EndScrollView();
		}

		private void Export() {
			foreach (RegexTodoSearch search in m_Searches) { search.ExportTSV(); }
		}

		public static InkTodoPrefs Load(DefaultAsset inkFile) {
			string inkPath = AssetDatabase.GetAssetPath(inkFile);
			string directory = Path.GetDirectoryName(inkPath);
			string prefsName = Path.GetFileNameWithoutExtension(inkPath) + ".asset";
			string prefsPath = Path.Combine(directory, prefsName);

			InkTodoPrefs prefs = AssetDatabase.LoadAssetAtPath<InkTodoPrefs>(prefsPath);

			if (!prefs) {
				prefs = CreateInstance<InkTodoPrefs>();
				AssetDatabase.CreateAsset(prefs, prefsPath);
				InkFileInfo inkInfo = new InkFileInfo(inkFile);
				prefs.m_InkFileAssets = new List<DefaultAsset> {inkFile};

				foreach (string includePath in inkInfo.Includes) {
					DefaultAsset includeAsset = AssetDatabase.LoadAssetAtPath<DefaultAsset>(includePath);
					if (includeAsset) { prefs.m_InkFileAssets.Add(includeAsset); }
				}

				prefs.m_Searches = new List<RegexTodoSearch> {new RegexTodoSearch()};
			}

			prefs.Analyze();

			return prefs;
		}
	}
}