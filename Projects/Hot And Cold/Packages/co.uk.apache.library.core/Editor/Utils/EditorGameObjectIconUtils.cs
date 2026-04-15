using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Apache.Core.Editor {
	public static class EditorGameObjectIconUtils {

		//-----------------------------------------------------------------------------------------
		// Type Definitions:
		//-----------------------------------------------------------------------------------------

		public enum LabelIcon {
			Gray,
			Blue,
			Teal,
			Green,
			Yellow,
			Orange,
			Red,
			Purple
		}

		public enum Icon {
			CircleGray,
			CircleBlue,
			CircleTeal,
			CircleGreen,
			CircleYellow,
			CircleOrange,
			CircleRed,
			CirclePurple,
			DiamondGray,
			DiamondBlue,
			DiamondTeal,
			DiamondGreen,
			DiamondYellow,
			DiamondOrange,
			DiamondRed,
			DiamondPurple
		}

		//-----------------------------------------------------------------------------------------
		// Private Fields:
		//-----------------------------------------------------------------------------------------

		private static GUIContent[] labelIcons;
		private static GUIContent[] largeIcons;

		//-----------------------------------------------------------------------------------------
		// Public Methods:
		//-----------------------------------------------------------------------------------------

		public static void SetIcon(GameObject gamo, LabelIcon icon) {
			if (labelIcons == null) {
				labelIcons = GetTextures("sv_label_", string.Empty, 0, 8);
			}
			SetIcon(gamo, labelIcons[(int)icon].image as Texture2D);
		}

		public static void SetIcon(GameObject gamo, Icon icon) {
			if (largeIcons == null) {
				largeIcons = GetTextures("sv_icon_dot", "_pix16_gizmo", 0, 16);
			}
			SetIcon(gamo, largeIcons[(int)icon].image as Texture2D);
		}

		//-----------------------------------------------------------------------------------------
		// Private Methods:
		//-----------------------------------------------------------------------------------------

		private static void SetIcon(GameObject gamo, Texture2D texture) {
			Type type = typeof(EditorGUIUtility);
			MethodInfo methodInfo = type.GetMethod("SetIconForObject", BindingFlags.NonPublic | BindingFlags.Static);
			methodInfo?.Invoke(null, new object[] { gamo, texture });
		}

		private static GUIContent[] GetTextures(string baseName, string postFix, int startIndex, int count) {
			GUIContent[] guiContentArray = new GUIContent[count];
			Type type = typeof(EditorGUIUtility);
			MethodInfo methodInfo = type.GetMethod("IconContent", BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(string) }, null);
			for (int i = 0; i < count; i++) {
				guiContentArray[i] = methodInfo?.Invoke(null, new object[] { baseName + (startIndex + i) + postFix }) as GUIContent;
			}
			return guiContentArray;
		}
	}
}