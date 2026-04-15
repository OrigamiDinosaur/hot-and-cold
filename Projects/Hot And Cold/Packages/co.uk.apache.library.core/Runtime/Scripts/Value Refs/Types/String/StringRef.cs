using System;

namespace Apache.Core {

	[Serializable]
	public class StringRef : ValueRef<string, StringContainer> {

		//-----------------------------------------------------------------------------------------
		// Operator Overrides:
		//-----------------------------------------------------------------------------------------

		public static implicit operator string(StringRef aRef) {
			return aRef.Val;
		}
	}
}