using System;

namespace Apache.Core {

	[Serializable]
	public class FloatRangeRef : ValueRef<FloatRange, FloatRangeContainer> {

		//-----------------------------------------------------------------------------------------
		// Operator Overrides:
		//-----------------------------------------------------------------------------------------

		public static implicit operator FloatRange(FloatRangeRef aRef) {
			return aRef.Val;
		}
	}
}