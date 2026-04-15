using System;
using UnityEngine;

namespace Apache.Core {

	[AttributeUsage(AttributeTargets.Field)]
	public class FloatRangeLimitsAttribute : PropertyAttribute {

		//-----------------------------------------------------------------------------------------
		// Public Properties:
		//-----------------------------------------------------------------------------------------

		public float Min { get; }
		public float Max { get; }

		//-----------------------------------------------------------------------------------------
		// Public Methods:
		//-----------------------------------------------------------------------------------------

		public FloatRangeLimitsAttribute(float min = 0, float max = 1) {
			Min = min;
			Max = max;
		}
	}
}