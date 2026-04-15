using UnityEditor;
using UnityEngine;

namespace Apache.Core.Editor {

	[CustomPropertyDrawer(typeof(SubheaderAttribute))]
	public class SubheaderDrawer : DecoratorDrawer {

		//-----------------------------------------------------------------------------------------
		// Constants:
		//-----------------------------------------------------------------------------------------

		private const float HEIGHT = 20f;

		//-----------------------------------------------------------------------------------------
		// Unity Lifecycle:
		//-----------------------------------------------------------------------------------------

		public override void OnGUI(Rect position) {

			position.y += 2f;
			position = EditorGUI.IndentedRect(position);

			// grab default GUI colour and colour our label in a light grey.
			Color defaultGuiColour = GUI.color;
			GUI.color = Colours.MID_LIGHTEST_GREY;

			// grab subheader attribute and draw it as a label.
			// ReSharper disable once UsePatternMatching
			SubheaderAttribute subheaderAttribute = attribute as SubheaderAttribute;
			if (subheaderAttribute != null) {
				GUI.Label(position, subheaderAttribute.Subheader, EditorStyles.miniBoldLabel);
			}

			// reinstate default GUI colour.
			GUI.color = defaultGuiColour;
		}

		//-----------------------------------------------------------------------------------------
		// Public Methods:
		//-----------------------------------------------------------------------------------------

		public override float GetHeight() {
			return HEIGHT;
		}
	}
}