using System.Linq;
using UnityEngine;

namespace Deviant.Utils {
	public class Editor<T> : UnityEditor.Editor where T : Object {
		private T m_TypedTarget;
		private T[] m_TypedTargets;

		protected T Target => m_TypedTarget ? m_TypedTarget : m_TypedTarget = target as T;
		protected T[] Targets => m_TypedTargets != null ? m_TypedTargets : m_TypedTargets = targets.Cast<T>().ToArray();
	}
}