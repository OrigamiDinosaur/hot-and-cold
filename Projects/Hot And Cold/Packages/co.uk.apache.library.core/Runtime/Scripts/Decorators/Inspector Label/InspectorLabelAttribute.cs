using System;
using UnityEngine;

namespace Apache.Core {

	[AttributeUsage(AttributeTargets.Field)]
	public class InspectorLabelAttribute : PropertyAttribute {

		//-----------------------------------------------------------------------------------------
		// Public Properties:
		//-----------------------------------------------------------------------------------------

		public InspectorLabelDisplayTypes DisplayType { get; }
		public string Label { get; }

		//-----------------------------------------------------------------------------------------
		// Constructors:
		//-----------------------------------------------------------------------------------------

		public InspectorLabelAttribute(string label) {
			Label = label;
		}

		public InspectorLabelAttribute(InspectorLabelDisplayTypes displayType) {
			DisplayType = displayType;
		}
	}
}