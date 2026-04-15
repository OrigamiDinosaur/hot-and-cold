using System;

namespace Apache.Core {

	[Serializable]
	public class LateralsRef : ValueRef<Laterals, LateralsContainer> {

		//-----------------------------------------------------------------------------------------
		// Operator Overrides:
		//-----------------------------------------------------------------------------------------

		public static implicit operator Laterals(LateralsRef aRef) {
			return aRef.Val;
		}
	}
}