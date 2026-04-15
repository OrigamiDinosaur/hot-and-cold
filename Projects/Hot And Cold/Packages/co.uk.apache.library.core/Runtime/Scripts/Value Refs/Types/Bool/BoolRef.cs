using System;

namespace Apache.Core {

	[Serializable]
	public class BoolRef : ValueRef<bool, BoolContainer> {

		//-----------------------------------------------------------------------------------------
		// Operator Overrides:
		//-----------------------------------------------------------------------------------------

		public static implicit operator bool(BoolRef aRef) {
			return aRef.Val;
		}
	}
}