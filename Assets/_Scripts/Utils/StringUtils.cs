using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace Deviant.Utils {
	public static class StringUtils {
		public static bool Contains(this string text, string search, StringComparison comparisonType) {
			if (search.IsNullOrEmpty()) { return true; }
			if (text.IsNullOrEmpty()) { return false; }
			return text.IndexOf(search, comparisonType) >= 0;
		}

		[ContractAnnotation("source:null => true")]
		public static bool IsNullOrEmpty(this string source) {
			return string.IsNullOrEmpty(source);
		}

		public static bool ContainsRegex(this string input, string pattern, RegexOptions options = RegexOptions.None) {
			return Regex.Match(input, pattern, options).Success;
		}

		public static string ReplaceRegex(this string input, string pattern, string replacement, RegexOptions options = RegexOptions.None) {
			return Regex.Replace(input, pattern, replacement, options);
		}

		public static string[] SplitRegex(this string input, string pattern, RegexOptions options = RegexOptions.None) { return Regex.Split(input, pattern, options); }

		public static string Replace(this string originalString, string oldValue, string newValue, StringComparison comparisonType) {
			int startIndex = 0;
			while (true) {
				startIndex = originalString.IndexOf(oldValue, startIndex, comparisonType);
				if (startIndex == -1) { break; }
				originalString = originalString.Substring(0, startIndex) + newValue + originalString.Substring(startIndex + oldValue.Length);
				startIndex += newValue.Length;
			}

			return originalString;
		}
	}
}