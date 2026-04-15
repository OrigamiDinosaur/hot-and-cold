using System;
using UnityEngine;

namespace Apache.Core {

	[AttributeUsage(AttributeTargets.Field)]
	public class FloatRangeSliderAttribute : PropertyAttribute {

		//-----------------------------------------------------------------------------------------
		// Public Properties:
		//-----------------------------------------------------------------------------------------

		public float Min { get; }
		public float Max { get; }

		//-----------------------------------------------------------------------------------------
		// Public Methods:
		//-----------------------------------------------------------------------------------------

		public FloatRangeSliderAttribute(float min = 0, float max = 1) {
			Min = min;
			Max = max;
		}
	}
}