using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Apache.Core {
	public class StaticBatchingHelper : ApacheComponent {

#if UNITY_EDITOR

		//-----------------------------------------------------------------------------------------
		// Constants:
		//-----------------------------------------------------------------------------------------

		public const string COMBINED_MESH_IDENTIFIER = "Combined Mesh";

		//-----------------------------------------------------------------------------------------
		// Public Methods:
		//-----------------------------------------------------------------------------------------

		[ApacheButton]
		public void LogUnbatchedMeshes() {

			// get all components with a mesh filter.
			MeshFilter[] meshFilters = FindObjectsByType<MeshFilter>();

			// create a list for offending game objects.
			List<UnbatchedMesh> unbatchedMeshes = new List<UnbatchedMesh>();

			// now loop through all mesh filters finding those which haven't been statically batched.
			foreach (MeshFilter meshFilter in meshFilters) {

				// grab the game object, continuing if it's disabled.
				GameObject gamo = meshFilter.gameObject;
				if (!gamo.activeInHierarchy) continue;

				// if there is no mesh renderer component, or it's disabled, continue.
				MeshRenderer meshRenderer = gamo.GetComponent<MeshRenderer>();
				if (meshRenderer == null || !meshRenderer.enabled) continue;

				// if the mesh name does not start with combined mesh identifier, add it to the list.
				string meshName = meshFilter.sharedMesh != null ? meshFilter.sharedMesh.name : meshFilter.mesh.name;
				if (!meshName.Contains(COMBINED_MESH_IDENTIFIER)) {
					unbatchedMeshes.Add(new UnbatchedMesh(meshName, gamo));
				}
			}

			// sort the list of non-static batched game objects.
			unbatchedMeshes.Sort();

			// now loop through and log offending gamos.
			foreach (UnbatchedMesh unbatchedMesh in unbatchedMeshes) {

				// log an error if it is static flagged but not batching, or a warning if it isn't flagged.
				if (unbatchedMesh.IsFlaggedForStaticBatching) {
					Debug.LogError(unbatchedMesh.DisplayName, unbatchedMesh.GameObject);
				}
				else {
					Debug.LogWarning(unbatchedMesh.DisplayName, unbatchedMesh.GameObject);
				}
			}
		}

		//-----------------------------------------------------------------------------------------
		// Structs:
		//-----------------------------------------------------------------------------------------

		private struct UnbatchedMesh : IComparable {

			//-----------------------------------------------------------------------------------------
			// Constants:
			//-----------------------------------------------------------------------------------------

			private const string DISPLAY_NAME_FORMAT = "{0} / {1}";

			//-----------------------------------------------------------------------------------------
			// Public Properties:
			//-----------------------------------------------------------------------------------------

			public GameObject GameObject { get; }

			public bool IsFlaggedForStaticBatching { get; }

			public string DisplayName => string.Format(DISPLAY_NAME_FORMAT, GameObject.name, meshName);

			//-----------------------------------------------------------------------------------------
			// Public Fields:
			//-----------------------------------------------------------------------------------------

			private readonly string meshName;

			//-----------------------------------------------------------------------------------------
			// Public Methods:
			//-----------------------------------------------------------------------------------------

			public UnbatchedMesh(string meshName, GameObject gameObject) : this() {

				this.meshName = meshName;
				GameObject = gameObject;

				// determine whether this game object is static flagged.
				StaticEditorFlags staticFlags = GameObjectUtility.GetStaticEditorFlags(GameObject);
				IsFlaggedForStaticBatching = ((staticFlags & StaticEditorFlags.BatchingStatic) == StaticEditorFlags.BatchingStatic);
			}

			public int CompareTo(object @object) {

				// if this isn't a static batched mesh, 
				if (!(@object is UnbatchedMesh)) return 0;
				UnbatchedMesh compareTo = (UnbatchedMesh)@object;

				// if either is flagged for static batching and not the other, give it sort priority.
				if (IsFlaggedForStaticBatching && !compareTo.IsFlaggedForStaticBatching) return -1;
				if (!IsFlaggedForStaticBatching && compareTo.IsFlaggedForStaticBatching) return 1;

				// otherwise use simple alphabetic sorting based on the mesh name.
				return string.Compare(DisplayName, compareTo.DisplayName, StringComparison.CurrentCulture);
			}
		}

#endif
	}
}