using System;

namespace Apache.Core {

	[Serializable]
	public class FloatRef : ValueRef<float, FloatContainer> {

		//-----------------------------------------------------------------------------------------
		// Operator Overrides:
		//-----------------------------------------------------------------------------------------

		public static implicit operator float(FloatRef floatRef) {
			return floatRef.Val;
		}
	}
}