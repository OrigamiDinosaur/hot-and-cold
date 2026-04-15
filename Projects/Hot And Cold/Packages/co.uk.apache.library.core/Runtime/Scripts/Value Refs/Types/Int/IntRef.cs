using System;

namespace Apache.Core {

	[Serializable]
	public class IntRef : ValueRef<int, IntContainer> {

		//-----------------------------------------------------------------------------------------
		// Operator Overrides
		//-----------------------------------------------------------------------------------------

		public static implicit operator int(IntRef intRef) {
			return intRef.Val;
		}
	}
}