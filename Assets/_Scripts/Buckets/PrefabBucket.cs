using System;
using System.Collections.Generic;
using System.Linq;
using Deviant.Utils;
using UnityEngine;

namespace Deviant.Commands {
	[CreateAssetMenu]
	public class PrefabBucket : GenericBucket<GameObject> {
		public event Action<GameObject> Added;
		public event Action<GameObject> Removed;

		private Dictionary<string, GameObject> m_PrefabLookup;
		private Dictionary<string, GameObject> m_InstanceLookup;

		private Dictionary<string, GameObject> PrefabLookup => m_PrefabLookup ?? (m_PrefabLookup = Items.Where(c => c).ToDictionary(c => c.name, StringComparer.OrdinalIgnoreCase));
		private Dictionary<string, GameObject> InstanceLookup => m_InstanceLookup ?? (m_InstanceLookup = new Dictionary<string, GameObject>(StringComparer.OrdinalIgnoreCase));

		public IEnumerable<GameObject> Prefabs => PrefabLookup.Values;
		public IEnumerable<GameObject> Instances => InstanceLookup.Values;
		
		public override string[] ItemNames => m_Items == null ? new string[0] : m_Items.Where(i => i != null).Select(i => i.name).ToArray();

		public virtual void Add(GameObject instance) {
			InstanceLookup[instance.name] = instance;
			Added?.Invoke(instance);
		}

		public virtual void Remove(GameObject instance) {
			InstanceLookup.Remove(instance.name);
			Removed?.Invoke(instance);
		}

		public GameObject Get(string prefabName) {
			if (prefabName.IsNullOrEmpty()) { return null; }
			InstanceLookup.TryGetValue(prefabName, out GameObject instance);
			return instance;
		}

		public GameObject GetOrCreate(string prefabName) {
			GameObject instance = Get(prefabName);
			if (instance) { return instance; }
			PrefabLookup.TryGetValue(prefabName, out GameObject prefab);
			if (prefab) {
				instance = Instantiate(prefab);
				Add(instance);
			}
			else { Debug.LogWarningFormat(this, "Couldn't find asset '{0}' in {1}", prefabName, GetType().Name); }
			
			return instance;
		}

		public void Destroy(string prefabName) {
			if (!Destroy(Get(prefabName))) { Debug.LogWarningFormat(this, "Couldn't remove '" + prefabName + "'. Instance not found."); }
		}

		public bool Destroy(GameObject instance) {
			if (!instance) { return false; }
			Remove(instance);
			Destroy(instance);
			return true;
		}

		public override bool HasItem(string itemName) { return Items.Any(item => item && item.name.Equals(itemName, StringComparison.OrdinalIgnoreCase)); }
	}
}