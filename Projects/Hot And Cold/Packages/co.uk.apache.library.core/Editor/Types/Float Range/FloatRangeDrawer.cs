using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Apache.Core.Editor {

	[CustomPropertyDrawer(typeof(FloatRange), true)]
	public class FloatRangeDrawer : PropertyDrawer {

		//-----------------------------------------------------------------------------------------
		// Constants:
		//-----------------------------------------------------------------------------------------

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

		private const float INDENT_OFFSET = 15;

		private const float SLIDER_VALUE_FIELD_WIDTH = 36;
		private const float SLIDER_VALUE_PADDING = 6;

		private const string MIN_MAX_VALUE_STRING_FORMAT = "0.##";

		//-----------------------------------------------------------------------------------------
		// Unity Lifecycle:
		//-----------------------------------------------------------------------------------------

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			
			// calulcate indentOffset.
			float indentOffset = INDENT_OFFSET * EditorGUI.indentLevel;

			// grab values used regardless of slider attribute.
			// extract existing min and max properties and values.
			SerializedProperty minProperty = property.FindPropertyRelative(MIN_PROPERTY_PATH);
			SerializedProperty maxProperty = property.FindPropertyRelative(MAX_PROPERTY_PATH);
			float min = minProperty.floatValue;
			float max = maxProperty.floatValue;

			// check if we are being constained by a constraint attribute.
			FloatRangeLimitsAttribute limits = (FloatRangeLimitsAttribute)fieldInfo.GetCustomAttributes(typeof(FloatRangeLimitsAttribute), true).FirstOrDefault();
			
			// if we have a constraint attribute use its values to clamp both min and max values.
			if (limits != null) {
				min = Mathf.Clamp(min, limits.Min, limits.Max);
				max = Mathf.Clamp(max, limits.Min, limits.Max);

				minProperty.floatValue = min;
				maxProperty.floatValue = max;
			}

			// check for a slider attribute.
			FloatRangeSliderAttribute slider =(FloatRangeSliderAttribute)fieldInfo.GetCustomAttributes(typeof(FloatRangeSliderAttribute), true).FirstOrDefault();

			// if there is no slider attribute then do default drawer...
			if (slider == null) {
				
				label = EditorGUI.BeginProperty(position, label, property);
				position = EditorGUI.PrefixLabel(position, label);

				// offset the position to counter indent levels.
				position.xMin -= (indentOffset) - LEFT_INITIAL_OFFSET;
				position.xMax += (indentOffset);

				// create style for labels.
				GUIStyle labelStyle = new GUIStyle(EditorStyles.miniBoldLabel) { alignment = TextAnchor.MiddleLeft };

				// create style for the float fields.
				GUIStyle floatFieldStyle = new GUIStyle(EditorStyles.textField) { alignment = TextAnchor.MiddleLeft };

				// calculate the size of the min and max label.
				float minLabelWidth = labelStyle.CalcSize(new GUIContent(MIN_VALUE_LABEL)).x;
				float maxLabelWidth = labelStyle.CalcSize(new GUIContent(MAX_VALUE_LABEL)).x;

				// calculate the size of the fields for the each label based on remaining width.
				float remainingWidth = position.width - (minLabelWidth + maxLabelWidth);
				float fieldWidth = remainingWidth / 2;

				// create a new rect for the min label.
				Rect minlabelRect = new Rect(
					position.xMin,
					position.y + LABEL_PADDING_FROM_TOP,
					minLabelWidth + indentOffset,
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
				EditorGUIUtility.labelWidth = MIN_LABEL_WIDTH + indentOffset;

				// N.B. this is where things get dirty...
				// If you want a slideable value via an inspector property's label you have to use an EditorGUI function that uses an in-built label.
				// However, you can't properly edit the style options of the inbuilt label. As such, we are rendering a label with our custom style
				// and then using a float field with a blank 'label'. The space of the blank label/EditorGUIUtility.labelWidth defines a space that
				// matches our existing label which is recognised by the float field as a dragHotZone.	This allows us the useful functionality of a
				// floatField as well as a custom style on a label. 
				minProperty.floatValue =
					EditorGUI.FloatField(minFieldRect, MIN_VALUE_BLANK_STRING, minProperty.floatValue, floatFieldStyle);
				
				// create a new rect for the max label.
				Rect maxlabelRect = new Rect(
					minFieldRect.xMax - indentOffset + MAX_LEFT_PADDING,
					position.y + LABEL_PADDING_FROM_TOP,
					maxLabelWidth + indentOffset,
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
				EditorGUIUtility.labelWidth = MAX_LABEL_WIDTH + indentOffset;

				// N.B dirty implementation, see above.
				// draw our float field.
				maxProperty.floatValue =
					EditorGUI.FloatField(maxFieldRect, MAX_VALUE_BLANK_STRING, maxProperty.floatValue, floatFieldStyle);
				
			}

			// ... otherwise draw the slider version of float range.
			else {
				
				// draw label left of range slider.
				label = EditorGUI.BeginProperty(position, label, property);
				position = EditorGUI.PrefixLabel(position, label);
				
				// offset the position to counter indent levels.
				position.xMin -= (indentOffset );

				// determine range based on whether a min max attribute is present, defaulting to zero to one if not.
				float rangeMin = 0;
				float rangeMax = 1;
				FloatRangeSliderAttribute range = (FloatRangeSliderAttribute)fieldInfo.GetCustomAttributes(typeof(FloatRangeSliderAttribute), true).FirstOrDefault();
				if (range != null) {
					rangeMin = range.Min;
					rangeMax = range.Max;

					// prevent ranges where min is greater than max.
					if (rangeMin > rangeMax) {
						rangeMin = rangeMax;
					}
				}

				// configure position for drawing min text field.
				float prevXMin = position.xMin;
				float prevXMax = position.xMax;
				position.xMax = position.xMin + SLIDER_VALUE_FIELD_WIDTH + indentOffset;

				// draw min value label left of the slider.
				EditorGUI.BeginChangeCheck();
				string minTextValue = EditorGUI.TextField(position, min.ToString(MIN_MAX_VALUE_STRING_FORMAT));
				if (EditorGUI.EndChangeCheck()) {

					// parse the edited value and ensure we're not in a situation where min is greater than max, then assign.
					if (float.TryParse(minTextValue, out min)) {
						if (min > max) {
							min = max;
						}
						minProperty.floatValue = Mathf.Clamp(min, rangeMin, rangeMax);
					}
				}

				// reset position for drawing max text field.
				position.xMax = prevXMax;
				position.xMin = prevXMax - SLIDER_VALUE_FIELD_WIDTH - indentOffset;

				// draw min value label left of the slider.
				EditorGUI.BeginChangeCheck();
				string maxTextValue = EditorGUI.TextField(position, max.ToString(MIN_MAX_VALUE_STRING_FORMAT));
				if (EditorGUI.EndChangeCheck()) {

					// parse the edited value and ensure we're not in a situation where max is less than min, then assign.
					if (float.TryParse(maxTextValue, out max)) {
						if (max < min) {
							max = min;
						}
						maxProperty.floatValue = Mathf.Clamp(max, rangeMin, rangeMax);
					}
				}

				// now slide position x min forward and x max back so the slider is in between the two text areas plus padding.
				position.xMin = prevXMin + SLIDER_VALUE_FIELD_WIDTH + SLIDER_VALUE_PADDING;
				position.xMax -= SLIDER_VALUE_FIELD_WIDTH + SLIDER_VALUE_PADDING ;

				// draw the slider and, if the value changes, update the serialised property.
				EditorGUI.BeginChangeCheck();
				EditorGUI.MinMaxSlider(position, ref min, ref max, rangeMin, rangeMax);
				if (EditorGUI.EndChangeCheck()) {
					minProperty.floatValue = Mathf.Clamp(min, rangeMin, rangeMax);
					maxProperty.floatValue = Mathf.Clamp(max, rangeMin, rangeMax);
				}

				EditorGUI.EndProperty();
			}
		}
	}
}