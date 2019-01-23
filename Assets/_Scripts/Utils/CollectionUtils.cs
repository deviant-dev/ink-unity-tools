using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Deviant.Utils {
	public static class CollectionUtils {
		public static void MoveUp<T>(this List<T> list, T item) {
			int index = list.IndexOf(item);

			if (index == 0) {
				Debug.LogWarning("Can't move above index '0'.");
				return;
			}

			list.Move(index, index - 1);
		}

		public static void MoveDown<T>(this List<T> list, T item) {
			int index = list.IndexOf(item);
			
			if (index == list.Count - 1) {
				Debug.LogWarning("Can't move past end of list.");
				return;
			}

			list.Move(index, index + 1);
		}

		private static void Move<T>(this IList<T> list, int oldIndex, int newIndex) {
			//if (oldIndex <= newIndex) { newIndex--; }
			T item = list[oldIndex];
			list.RemoveAt(oldIndex);
			list.Insert(newIndex, item);
		}
	}
}