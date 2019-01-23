using UnityEditor;
using UnityEngine;

namespace Deviant.Commands {
	[CustomEditor(typeof(PrefabBucket), true)]
	public class PrefabBucketEditor : GenericBucketEditor<PrefabBucket, GameObject> {
		public override void OnInspectorGUI() {
			base.OnInspectorGUI();

			if (Application.isPlaying) {
				GUI.enabled = false;

				GUILayout.Label("Prefabs:");
				foreach (GameObject instance in Target.Prefabs) { EditorGUILayout.ObjectField(instance, typeof(GameObject), false); }

				EditorGUILayout.Space();

				GUILayout.Label("Instances:");
				foreach (GameObject instance in Target.Instances) { EditorGUILayout.ObjectField(instance, typeof(GameObject), true); }
			}
		}
	}
}