using System.Linq;
using UnityEngine;

namespace Deviant.Commands {
	public abstract class GenericBucket<T> : BaseBucket where T : class {
		[SerializeField] protected T[] m_Items;

		public T[] Items => m_Items ?? (m_Items = new T[0]);

		public override bool HasItem(string itemName) { return Items.Any(item => item != null && item.ToString() == itemName); }
	}
}