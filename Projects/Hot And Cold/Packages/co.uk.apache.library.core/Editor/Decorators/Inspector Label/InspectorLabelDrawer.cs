using UnityEditor;
using UnityEngine;

namespace Apache.Core.Editor {

	[CustomPropertyDrawer(typeof(InspectorLabelAttribute), true)]
	public class InspectorLabelPropertyDrawer : PropertyDrawer {

		//-----------------------------------------------------------------------------------------
		// Constants:
		//-----------------------------------------------------------------------------------------

		private static readonly string[] BOOL_PREFIXES_TO_REMOVE = { "Is", "Should", "Can", "Has" };
		private static readonly string[] BOOL_PREFIX_SUFFIXES_NOT_TO_REMOVE = { "nt", " Not" };

		//-----------------------------------------------------------------------------------------
		// Unity Lifecycle:
		//-----------------------------------------------------------------------------------------

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

			InspectorLabelAttribute attr = (InspectorLabelAttribute)attribute;

			switch (attr.DisplayType) {

				// if it's a custom label, just directly set the text to that custom label.
				case InspectorLabelDisplayTypes.CustomLabel:
					label.text = attr.Label;
					break;

				case InspectorLabelDisplayTypes.Bool:

					// grab display name.
					string text = property.displayName;

					// remove common bool prefixes like "should"; that meaning is somewhat implicit in the editor.
					foreach (string removePrefix in BOOL_PREFIXES_TO_REMOVE) {
						if (text.StartsWith(removePrefix)) {

							// continue if we match a prefix suffix we shouldn't remove.
							bool shouldNotRemove = false;
							foreach (string dontRemovePrefixSuffix in BOOL_PREFIX_SUFFIXES_NOT_TO_REMOVE) {
								if (text.StartsWith(removePrefix + dontRemovePrefixSuffix)) {
									shouldNotRemove = true;
								}
							}

							if (shouldNotRemove) continue;

							// simply substring away the prefix and assign text.
							text = text.Substring(removePrefix.Length, text.Length - removePrefix.Length);
							break;
						}
					}

					// remove any spaces which might have been left on the beginning.
					if (text.StartsWith(" ")) {
						text = text.Substring(1, text.Length - 1);
					}

					label.text = text;
					break;
			}

			EditorGUI.PropertyField(position, property, label);
		}
	}
}