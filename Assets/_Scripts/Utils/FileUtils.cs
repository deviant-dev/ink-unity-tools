using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Deviant.Utils {
	public static class FileUtils {
		public static void CreateFoldersFor(string path) {
			if (path.IsNullOrEmpty()) {
				Debug.LogWarning("Can't make a directory for an empty path.");
				return;
			}

			string folder = Path.GetDirectoryName(path);

			if (folder.IsNullOrEmpty() || Directory.Exists(folder)) { return; }

			Directory.CreateDirectory(folder);
		}
	}
}