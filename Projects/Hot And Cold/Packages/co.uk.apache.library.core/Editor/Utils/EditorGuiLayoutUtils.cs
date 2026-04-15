using Apache.Core;
using UnityEditor;
using UnityEngine;

public class EditorGuiLayoutUtils : ApacheComponent {

	//-----------------------------------------------------------------------------------------
	// Constants:
	//-----------------------------------------------------------------------------------------

	#region // FloatRange

	private const string MIN_PROPERTY_PATH = "min";
	private const string MAX_PROPERTY_PATH = "max";

	private const string MIN_VALUE_LABEL = "min";
	private const string MAX_VALUE_LABEL = "max";

	private const float LEFT_INITIAL_OFFSET = -3f;
	private const float LABEL_PADDING_FROM_TOP = -1f;

	private const float MIN_PADDING = 0.5f;
	private const float MIN_LABEL_WIDTH = 22f;
	private const string MIN_VALUE_BLANK_STRING = "     ";

	private const float MAX_LEFT_PADDING = 2.5f;
	private const float MAX_LABEL_WIDTH = 27f;
	private const string MAX_VALUE_BLANK_STRING = "      ";

	#endregion

	#region // Vector4Int

	private const float XYZW_LABEL_WIDTH = 15;

	#endregion
	
	//-----------------------------------------------------------------------------------------
	// Public Methods:
	//-----------------------------------------------------------------------------------------

	public static FloatRange FloatRangeField(string label, FloatRange value) {

		float min = value.Min;
		float max = value.Max;

		// create a rect proportionate to our current editor window.
		Rect position = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight);

		// create label for this property and adjust position.
		position = EditorGUI.PrefixLabel(position, new GUIContent(label));

		// shunt our rect for the left offset.
		position.xMin += LEFT_INITIAL_OFFSET;

		// create style for labels.
		GUIStyle labelStyle = new GUIStyle(EditorStyles.miniBoldLabel) { alignment = TextAnchor.MiddleLeft };

		// create style for the float fields.
		GUIStyle floatFieldStyle = new GUIStyle(EditorStyles.textField) { alignment = TextAnchor.MiddleLeft };

		// calculate the size of the min and max label.
		float minLabelWidth = labelStyle.CalcSize(new GUIContent(MIN_PROPERTY_PATH)).x;
		float maxLabelWidth = labelStyle.CalcSize(new GUIContent(MAX_PROPERTY_PATH)).x;

		// calculate the size of the fields for the each label based on remaining width.
		float remainingWidth = position.width - (minLabelWidth + maxLabelWidth);
		float fieldWidth = remainingWidth / 2;

		// create a new rect for the min label.
		Rect minlabelRect = new Rect(
			position.xMin,
			position.y + LABEL_PADDING_FROM_TOP,
			minLabelWidth,
			position.height);

		// N.B. change GUI color to draw the label as changing the color of the labelStyle appears not to do anything in editor code.
		// grab default GUI colour and colour our label in a light grey.
		Color defaultGUIColour = GUI.color;
		GUI.color = Colours.MID_LIGHTEST_GREY;

		// draw min label.
		EditorGUI.LabelField(minlabelRect, MIN_VALUE_LABEL, labelStyle);

		// return our GUI color to default.
		GUI.color = defaultGUIColour;

		// create a new rect for the min text field.
		Rect minFieldRect = new Rect(
			position.xMin,
			position.y,
			minLabelWidth + fieldWidth - MIN_PADDING - MAX_LEFT_PADDING,
			minlabelRect.height);

		// change label width for the min 'label' on our float field.
		EditorGUIUtility.labelWidth = MIN_LABEL_WIDTH;

		// N.B. this is where things get dirty.
		// If you want a slideable value via an inspector properties label you have to use an EditorGUI function that uses an in-built label,
		// however you can't properly edit the style options of the in-built label. As such, we are rendering a label with our custom style
		// and then using a float field with a blank 'label'. The space of the blank label/EDEditorGUIUtility.labelWidth defines a space that
		// matches our existing label which is recognised by the float field as a dragHotZone. This allows us the useful functionality of a
		// floatField as well as a custom style on a label. 
		min = EditorGUI.FloatField(minFieldRect, MIN_VALUE_BLANK_STRING, min, floatFieldStyle);

		// create a new rect for the max label.
		Rect maxlabelRect = new Rect(
			minFieldRect.xMax + MAX_LEFT_PADDING,
			position.y + LABEL_PADDING_FROM_TOP,
			maxLabelWidth,
			position.height);

		// change the GUI color again, we have already stored the default colour.
		GUI.color = Colours.MID_LIGHTEST_GREY;

		// draw min label.
		EditorGUI.LabelField(maxlabelRect, MAX_VALUE_LABEL, labelStyle);

		// return to default GUI colour.
		GUI.color = defaultGUIColour;

		// create a new rect for the max text field.
		Rect maxFieldRect = new Rect(
			maxlabelRect.x,
			position.y,
			maxLabelWidth + fieldWidth,
			minlabelRect.height);


		// change label width for the max 'label' on our float field.
		EditorGUIUtility.labelWidth = MAX_LABEL_WIDTH;

		// N.B dirty implementation, see above.
		// draw our float field.
		max = EditorGUI.FloatField(maxFieldRect, MAX_VALUE_BLANK_STRING, max, floatFieldStyle);

		return new FloatRange(min, max);
	}

	public static Vector4Int Vector4IntField(string label, Vector4Int value) {

		int x = value.x;
		int y = value.y;
		int z = value.z;
		int w = value.w;

		// create a rect proportionate to our current editor window.
		Rect position = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight);

		// create label for this property and adjust position.
		position = EditorGUI.PrefixLabel(position, new GUIContent(label));

		// shunt our rect for the left offset.
		position.xMin += LEFT_INITIAL_OFFSET;

		// calculate our width per Vector field.
		float widthPerFloatField = position.width / 4;

		// store the default label width so that we can restore it at the end.
		float defaultLabelWidth = EditorGUIUtility.labelWidth;

		// adjust our label width
		EditorGUIUtility.labelWidth = XYZW_LABEL_WIDTH;

		// create a rect for our fields to draw at.
		Rect vectorFieldRect = position;
		vectorFieldRect.xMax = vectorFieldRect.xMin + widthPerFloatField;

		// draw our x value.
		x = EditorGUI.IntField(vectorFieldRect, new GUIContent("X"), x);

		// adjust our rect to new position.
		vectorFieldRect.xMin = vectorFieldRect.xMax;
		vectorFieldRect.xMax = vectorFieldRect.xMin + widthPerFloatField;

		// draw our y value.
		y = EditorGUI.IntField(vectorFieldRect, new GUIContent("Y"), y);

		// adjust our rect to new position.
		vectorFieldRect.xMin = vectorFieldRect.xMax;
		vectorFieldRect.xMax = vectorFieldRect.xMin + widthPerFloatField;

		// draw our z value.
		z = EditorGUI.IntField(vectorFieldRect, new GUIContent("Z"), z);

		// adjust our rect to new position.
		vectorFieldRect.xMin = vectorFieldRect.xMax;
		vectorFieldRect.xMax = vectorFieldRect.xMin + widthPerFloatField;

		// draw our w value.
		w = EditorGUI.IntField(vectorFieldRect, new GUIContent("W"), w);

		// before we exit our restore label width.
		EditorGUIUtility.labelWidth = defaultLabelWidth;

		return new Vector4Int(x, y, z, w);
	}

	public static Vector4 Vector4FieldFullWidth(string label, Vector4 value) {

		float x = value.x;
		float y = value.y;
		float z = value.z;
		float w = value.w;

		// create a rect proportionate to our current editor window.
		Rect position = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight);

		// create label for this property and adjust position.
		position = EditorGUI.PrefixLabel(position, new GUIContent(label));

		// shunt our rect for the left offset.
		position.xMin += LEFT_INITIAL_OFFSET;

		// calculate our width per Vector field.
		float widthPerFloatField = position.width / 4;

		// store the default label width so that we can restore it at the end.
		float defaultLabelWidth = EditorGUIUtility.labelWidth;

		// adjust our label width
		EditorGUIUtility.labelWidth = XYZW_LABEL_WIDTH;

		// create a rect for our fields to draw at.
		Rect vectorFieldRect = position;
		vectorFieldRect.xMax = vectorFieldRect.xMin + widthPerFloatField;

		// draw our x value.
		x = EditorGUI.FloatField(vectorFieldRect, new GUIContent("X"), x);

		// adjust our rect to new position.
		vectorFieldRect.xMin = vectorFieldRect.xMax;
		vectorFieldRect.xMax = vectorFieldRect.xMin + widthPerFloatField;

		// draw our y value.
		y = EditorGUI.FloatField(vectorFieldRect, new GUIContent("Y"), y);

		// adjust our rect to new position.
		vectorFieldRect.xMin = vectorFieldRect.xMax;
		vectorFieldRect.xMax = vectorFieldRect.xMin + widthPerFloatField;

		// draw our z value.
		z = EditorGUI.FloatField(vectorFieldRect, new GUIContent("Z"), z);

		// adjust our rect to new position.
		vectorFieldRect.xMin = vectorFieldRect.xMax;
		vectorFieldRect.xMax = vectorFieldRect.xMin + widthPerFloatField;

		// draw our w value.
		w = EditorGUI.FloatField(vectorFieldRect, new GUIContent("W"), w);

		// before we exit our restore label width.
		EditorGUIUtility.labelWidth = defaultLabelWidth;

		return new Vector4(x, y, z, w);
	}

	public static Vector2 Vector2FieldFullWidth(string label, Vector2 value) {

		// grab our float values from the vector2 value.
		float x = value.x;
		float y = value.y;

		// create a rect proportionate to our current editor window.
		Rect position = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight);

		// create label for this property and adjust position. 
		position = EditorGUI.PrefixLabel(position, new GUIContent(label));

		// shunt our rect for the left offset.
		position.xMin += LEFT_INITIAL_OFFSET;

		// calculate our width per Vector field.
		float widthPerFloatField = position.width / 2;

		// store the default label width so that we can restore it at the end.
		float defualtLabelWidth = EditorGUIUtility.labelWidth;

		// adjust our label width.
		EditorGUIUtility.labelWidth = XYZW_LABEL_WIDTH;

		// create a rect for our fields to draw at.
		Rect vectorFieldRect = position;
		vectorFieldRect.xMax = vectorFieldRect.xMin + widthPerFloatField;

		// draw our x value.
		x = EditorGUI.FloatField(vectorFieldRect, new GUIContent("X"), x);

		// adjust our rect to new position.
		vectorFieldRect.xMin = vectorFieldRect.xMax;
		vectorFieldRect.xMax = vectorFieldRect.xMin + widthPerFloatField;

		// draw our y value.
		y = EditorGUI.FloatField(vectorFieldRect, new GUIContent("Y"), y);

		// before we exit out restore our label width.
		EditorGUIUtility.labelWidth = defualtLabelWidth;

		return new Vector2(x, y);
	}

	public static Vector2Int Vector2IntFieldFullWidth(string label, Vector2Int value) {

		// grab our float values from the vector2 value.
		int x = value.x;
		int y = value.y;

		// create a rect proportionate to our current editor window.
		Rect position = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight);

		// create label for this property and adjust position. 
		position = EditorGUI.PrefixLabel(position, new GUIContent(label));

		// shunt our rect for the left offset.
		position.xMin += LEFT_INITIAL_OFFSET;

		// calculate our width per Vector field.
		float widthPerFloatField = position.width / 2;

		// store the default label width so that we can restore it at the end.
		float defualtLabelWidth = EditorGUIUtility.labelWidth;

		// adjust our label width.
		EditorGUIUtility.labelWidth = XYZW_LABEL_WIDTH;

		// create a rect for our fields to draw at.
		Rect vectorFieldRect = position;
		vectorFieldRect.xMax = vectorFieldRect.xMin + widthPerFloatField;

		// draw our x value.
		x = EditorGUI.IntField(vectorFieldRect, new GUIContent("X"), x);

		// adjust our rect to new position.
		vectorFieldRect.xMin = vectorFieldRect.xMax;
		vectorFieldRect.xMax = vectorFieldRect.xMin + widthPerFloatField;

		// draw our y value.
		y = EditorGUI.IntField(vectorFieldRect, new GUIContent("Y"), y);

		// before we exit out restore our label width.
		EditorGUIUtility.labelWidth = defualtLabelWidth;

		return new Vector2Int(x, y);
	}
}