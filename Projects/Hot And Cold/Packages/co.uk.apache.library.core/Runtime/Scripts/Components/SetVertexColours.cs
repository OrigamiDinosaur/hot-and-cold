using UnityEngine;

namespace Apache.Core {
	public class SetVertexColours : ApacheComponent {

		//-----------------------------------------------------------------------------------------
		// Constants:
		//-----------------------------------------------------------------------------------------

		private const string VERTEX_COLOURED_MESH_SUFFIX = " [Vertex Coloured]";

		//-----------------------------------------------------------------------------------------
		// Inspector Variables:
		//-----------------------------------------------------------------------------------------

		[ApacheSpace]

		[SerializeField] protected Color colour;

		//-----------------------------------------------------------------------------------------
		// Private Fields:
		//-----------------------------------------------------------------------------------------

		private MeshFilter meshFilter;

		//-----------------------------------------------------------------------------------------
		// Public Methods:
		//-----------------------------------------------------------------------------------------

		public void SetVertexColour(Color aColour) {
			colour = aColour;
			SetVertexColour();
		}

		//-----------------------------------------------------------------------------------------
		// Public Methods - Buttons:
		//-----------------------------------------------------------------------------------------

		[ApacheButton]
		public void ClearVertexColour() {

			// to clear vertex colours, simply set to an empty colour array.
			Mesh mesh = GetOrCloneAndAssignSharedMesh();
			mesh.colors = new Color[0];
		}

		//-----------------------------------------------------------------------------------------
		// Private Methods - Buttons:
		//-----------------------------------------------------------------------------------------

		[ApacheButton]
		private void SetVertexColour() {

			Mesh mesh = GetOrCloneAndAssignSharedMesh();
			Vector3[] vertices = mesh.vertices;

			// create new colours array of our given colour.
			Color[] colours = new Color[vertices.Length];
			for (int i = 0; i < colours.Length; i++) {
				colours[i] = colour;
			}

			// assign the array of colours to the mesh.
			mesh.colors = colours;
		}

		//-----------------------------------------------------------------------------------------
		// Private Properties:
		//-----------------------------------------------------------------------------------------

		// Either gets the shared mesh if it is safe to edit or clones and assigns a copy of the shared mesh enabling safe vertex colouring.
		private Mesh GetOrCloneAndAssignSharedMesh() {

			// grab mesh filter if we don't have it already.
			if (meshFilter == null) {
				meshFilter = GetComponent<MeshFilter>();
			}

			// grab the shared mesh, and if it has already been vertex coloured (and therefore cloned and given the name suffix), use it.
			Mesh mesh = meshFilter.sharedMesh;
			if (mesh.name.Contains(VERTEX_COLOURED_MESH_SUFFIX)) {
				return mesh;
			}

			// otherwise clone the shared mesh and give it the name suffix.
			Mesh clonedMesh = Instantiate(mesh);
			clonedMesh.name = mesh.name + VERTEX_COLOURED_MESH_SUFFIX;

			meshFilter.sharedMesh = clonedMesh;
			return clonedMesh;
		}
	}
}