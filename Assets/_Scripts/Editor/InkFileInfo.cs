using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Deviant.Utils;
using UnityEditor;
using UnityEngine;

namespace Deviant.InkTodo {
	[Serializable]
	public class InkFileInfo {
		[SerializeField] private DefaultAsset m_InkAsset;
		[SerializeField] private string[] m_Lines;
		[SerializeField] private string m_Path;
		[SerializeField] private string m_DisplayPath;
		[SerializeField] private string[] m_Includes;
		[SerializeField] private string m_Directory;

		public DefaultAsset InkAsset => m_InkAsset ? m_InkAsset : m_InkAsset = AssetDatabase.LoadAssetAtPath<DefaultAsset>(m_Path);

		public string Path => m_Path ?? (m_Path = AssetDatabase.GetAssetPath(m_InkAsset));

		public string DisplayPath => m_DisplayPath;

		private string Directory => m_Directory ?? (m_Directory = System.IO.Path.GetDirectoryName(Path).Replace('\\', '/'));

		public string[] Lines => m_Lines ?? (m_Lines = Path.IsNullOrEmpty() ? null : File.ReadAllText(Path).SplitRegex(@"\r?\n"));

		public string[] Includes => m_Includes ?? (m_Includes = GetIncludes());

		private string[] GetIncludes() {
			IEnumerable<string> includeLines = Lines.Where(l => l.StartsWith("INCLUDE"));
			return includeLines.Select(l => $"{Directory}/{l.ReplaceRegex(@"^INCLUDE\s+", "")}".Replace('\\', '/')).ToArray();
		}

		public InkFileInfo() { }

		public InkFileInfo(DefaultAsset inkAsset) {
			m_InkAsset = inkAsset;
			m_DisplayPath = inkAsset.name;
		}

		public InkFileInfo(string path, string rootDirectory) {
			m_Path = path.Replace("\\", "/");
			m_DisplayPath = path.Replace(rootDirectory.TrimEnd('/') + "/", "", StringComparison.OrdinalIgnoreCase);
		}
	}
} 