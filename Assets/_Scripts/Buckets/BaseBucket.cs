using UnityEngine;

namespace Deviant.Commands {
	public abstract class BaseBucket : ScriptableObject {
		public abstract string[] ItemNames { get; }

		public abstract bool HasItem(string itemName);

		public virtual void Subscribe() { }
		public virtual void Unsubscribe() { }
	}
}