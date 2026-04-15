using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Apache.Core.Editor {

	[CustomEditor(typeof(ApacheComponent), true, isFallback = true)]
	[CanEditMultipleObjects]
	public class ApacheComponentEditor : UnityEditor.Editor {

		//-----------------------------------------------------------------------------------------
		// Constants:
		//-----------------------------------------------------------------------------------------

		private const string AUTO_REF_UNDO_GROUP_NAME = "Auto Referencing";

		//-----------------------------------------------------------------------------------------
		// Private Fields:
		//-----------------------------------------------------------------------------------------

		private readonly ApacheButtonAttributeHelper buttonHelper = new ApacheButtonAttributeHelper();

		//-----------------------------------------------------------------------------------------
		// Unity Lifecycle:
		//-----------------------------------------------------------------------------------------

		protected virtual void OnEnable() {

			// init button helper before drawing Apache buttons.
			buttonHelper.Init(target);

			// below we handle ref attributes but we don't want to update them if playing, so back out if so.
			if (Application.isPlaying) return;
			
			// get all public and non-public instance fields.
			FieldInfo[] fields = target.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

			// start a new undo for auto references.
			Undo.SetCurrentGroupName(AUTO_REF_UNDO_GROUP_NAME);
			int undoGroupId = Undo.GetCurrentGroup();

			// loop through, and for each field with a ref attribute, update refs.
			foreach (FieldInfo fieldInfo in fields) {

				object[] attributes = fieldInfo.GetCustomAttributes(typeof(RefAttribute), false);

				foreach (object attribute in attributes) {

					// if this is an asset ref attribute, update assets at asset path.
					// N.B. we do this because we need to access assets from within an Editor directory.
					AssetRefAttribute refAttribute = attribute as AssetRefAttribute;
					refAttribute?.UpdateAssetsAtAssetPath(AssetRefAttributeHelper.LoadAllAssetsAtPath(refAttribute.Path));

					((RefAttribute)attribute).UpdateRefs(target, fieldInfo);
				}
			}

			// collapse auto referencing undo group into single undo.
			Undo.CollapseUndoOperations(undoGroupId);
		}

		public override void OnInspectorGUI() {
			base.OnInspectorGUI();

			// N.B. separating this out enables derived classes to call just this classes implementation without also calling
			// our base, which does all default drawing, which we may not want.
			OnInspectorGuiBase();
		}

		//-----------------------------------------------------------------------------------------
		// Protected Methods:
		//-----------------------------------------------------------------------------------------

		protected virtual void OnInspectorGuiBase() {
			buttonHelper.DrawButtons();
		}
	}
}