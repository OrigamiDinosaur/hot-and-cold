using Apache.Core.Extensions;
using UnityEngine;

namespace Apache.Core {
	public class BlendShapeRotationMap : ApacheComponent {

		//-----------------------------------------------------------------------------------------
		// Constants:
		//-----------------------------------------------------------------------------------------

		private static readonly FloatRange DEFAULT_ROTATION_RANGE = new FloatRange(0, 90);
		private static readonly FloatRange DEFAULT_VALUE_RANGE    = new FloatRange(0, 100);
		private static readonly AnimationCurve DEFAULT_MAP_CURVE = AnimationCurve.Linear(0, 0, 1, 1);

		//-----------------------------------------------------------------------------------------
		// Type Definitions:
		//-----------------------------------------------------------------------------------------

		public enum Axes {
			X,
			NegativeX,
			Y,
			NegativeY,
			Z,
			NegativeZ
		}

		//-----------------------------------------------------------------------------------------
		// Inspector Variables:
		//-----------------------------------------------------------------------------------------

		[Header("Blend Shape")]

		[SerializeField] protected SkinnedMeshRenderer skinnedMeshRenderer;
		[SerializeField] protected string blendShapeName;

		[Header("Rotation")]

		[SerializeField] protected Axes axis;
		[SerializeField] protected FloatRange rotationRange = DEFAULT_ROTATION_RANGE;
		[SerializeField] protected FloatRange valueRange = DEFAULT_VALUE_RANGE;
		[SerializeField] protected AnimationCurve mapCurve = DEFAULT_MAP_CURVE;

		//-----------------------------------------------------------------------------------------
		// Private Fields:
		//-----------------------------------------------------------------------------------------

		private int blendShapeIndex;

		private Quaternion defaultLocalRotation;

		private float prevBlendShapeValue;

		//-----------------------------------------------------------------------------------------
		// Unity Lifecycle:
		//-----------------------------------------------------------------------------------------

		protected void Awake() {

			// error if we don't have a skinned mesh renderer.
			if (skinnedMeshRenderer == null) {
				Debug.LogError("Please select a Skinned Mesh Renderer.");
				return;
			}

			// get the ID of our current blend shape name, erroring if there is a problem.
			blendShapeIndex = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(blendShapeName);
			if (blendShapeIndex == -1) {
				Debug.LogError($"Blend shape with name \"{ blendShapeName }\" not found.");
			}

			// grab default local rotation, from which we'll determine offsets.
			defaultLocalRotation = transform.localRotation;
		}

		protected void Update() {

			// back out if we have a bad blend shape ID.
			if (blendShapeIndex == -1) return;

			// work out the delta rotation from our default.
			Quaternion deltaRotation = Quaternion.Inverse(defaultLocalRotation) * transform.localRotation;

			// work out delta angle based on axis.
			float deltaAngle = 0;
			switch (axis) {
				case Axes.X:
				case Axes.NegativeX:
					deltaAngle = deltaRotation.GetPitch();
					break;
				case Axes.Y:
				case Axes.NegativeY:
					deltaAngle = deltaRotation.GetYaw();
					break;
				case Axes.Z:
				case Axes.NegativeZ:
					deltaAngle = deltaRotation.GetRoll();
					break;
			}

			// invert delta if we have a minus axis.
			if (IsNegativeAxis(axis)) {
				deltaAngle *= -1;
			}

			// work out blend shape value by inverse lerping it in rotation range and then lerping it on value range.
			float rotationT = rotationRange.InverseLerp(deltaAngle);
			float curvedRotationT = mapCurve.Evaluate(rotationT);
			float newBlendShapeValue = valueRange.Lerp(curvedRotationT);

			// if the value is unchanged, back out, otherwise set and persist.
			if (newBlendShapeValue == prevBlendShapeValue) return;
			skinnedMeshRenderer.SetBlendShapeWeight(blendShapeIndex, newBlendShapeValue);
			prevBlendShapeValue = newBlendShapeValue;
		}

#if UNITY_EDITOR
		protected void OnValidate() {

			// if we don't have a mesh renderer, try and grab one from parent.
			// N.B. this also handles selecting or cleaning up the blend shape name.
			if (skinnedMeshRenderer == null) {
				ToggleParentSkinnedMeshRenderer();
				return;
			}

			// N.B. in Unity 2018.2.20f1, trying to find a blend shape by name when name is a null string, causes a hard Unity crash,
			// so clean up if necessary to prevent that.
			if (blendShapeName == null) {
				blendShapeName = string.Empty;
			}

			// if we have an invalid blend shape name (after changing skinned mesh renderer, for instance), toggle it.
			if (skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(blendShapeName) == -1) {
				ToggleBlendShapeName();
			}
		}
#endif

		//-----------------------------------------------------------------------------------------
		// Private Methods:
		//-----------------------------------------------------------------------------------------

		private static bool IsNegativeAxis(Axes axis) {
			switch (axis) {
				case Axes.NegativeX:
				case Axes.NegativeY:
				case Axes.NegativeZ:
					return true;
				default:
					return false;
			}
		}

		//-----------------------------------------------------------------------------------------
		// Editor Methods:
		//-----------------------------------------------------------------------------------------

#if UNITY_EDITOR
		[ApacheButton]
		protected void ToggleParentSkinnedMeshRenderer() {

			// grab parent skinned mesh renderers, backing out if we don't find any.
			SkinnedMeshRenderer[] skinnedMeshRenderers = transform.root.GetComponentsInChildren<SkinnedMeshRenderer>();
			if (skinnedMeshRenderers.IsNullOrEmpty()) {
				ToggleBlendShapeName();
				return;
			}

			// if we only have one skinned mesh render in parent, or we don't yet have a skinned mesh, use the first.
			if (skinnedMeshRenderers.Length == 1 || skinnedMeshRenderer == null) {
				skinnedMeshRenderer = skinnedMeshRenderers[0];
				ToggleBlendShapeName();
				return;
			}

			// otherwise loop through parents until we find our current.
			SkinnedMeshRenderer currSkinedMeshRenderer = skinnedMeshRenderer;
			for (int i = 0; i < skinnedMeshRenderers.Length; i++) {
				if (skinnedMeshRenderers[i] != currSkinedMeshRenderer) continue;

				// we found our current, so use next or first.
				skinnedMeshRenderer = (i + 1 < skinnedMeshRenderers.Length) ? skinnedMeshRenderers[i + 1] : skinnedMeshRenderers[0];
				ToggleBlendShapeName();
				return;
			}

			// if we still don't have one, just use first.
			skinnedMeshRenderer = skinnedMeshRenderers[0];
			ToggleBlendShapeName();
		}

		[ApacheButton]
		protected void ToggleBlendShapeName() {

			// back out if we don't have a skinned mesh renderer.
			if (skinnedMeshRenderer == null) {
				blendShapeName = string.Empty;
				return;
			}

			// back out if we don't have blend shapes on our mesh.
			Mesh sharedMesh = skinnedMeshRenderer.sharedMesh;
			int numBlendShapes = sharedMesh.blendShapeCount;
			if (numBlendShapes == 0) {
				blendShapeName = string.Empty;
				return;
			}

			// we don't want to try to get blend shape index with a bad name, so use the first blend shape name if that's the case.
			if (blendShapeName.IsNullOrWhitespace() || numBlendShapes == 1) {
				blendShapeName = sharedMesh.GetBlendShapeName(0);
				return;
			}

			// get the index based on the current blend shape name, and if it's an invalid index, just use the first.
			int currIndex = sharedMesh.GetBlendShapeIndex(blendShapeName);
			if (currIndex == -1) {
				blendShapeName = sharedMesh.GetBlendShapeName(0);
				return;
			}

			// work out the next blend shape index (which might roll around to the first) and use that.
			int nextIndex = (currIndex == numBlendShapes - 1) ? 0 : ++currIndex;
			blendShapeName = sharedMesh.GetBlendShapeName(nextIndex);
		}
#endif
	}
}