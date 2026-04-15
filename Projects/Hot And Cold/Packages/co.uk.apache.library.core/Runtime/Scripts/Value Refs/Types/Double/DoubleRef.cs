using System;

namespace Apache.Core {

	[Serializable]
	public class DoubleRef : ValueRef<double, DoubleContainer> {

		//-----------------------------------------------------------------------------------------
		// Operator Overrides:
		//-----------------------------------------------------------------------------------------

		public static implicit operator double(DoubleRef aRef) {
			return aRef.Val;
		}
	}
}