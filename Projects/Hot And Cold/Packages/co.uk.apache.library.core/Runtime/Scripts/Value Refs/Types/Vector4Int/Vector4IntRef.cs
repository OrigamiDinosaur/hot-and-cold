using System;

namespace Apache.Core {

	[Serializable]
	public class Vector4IntRef : ValueRef<Vector4Int, Vector4IntContainer> {

		//-----------------------------------------------------------------------------------------
		// Operator Overrides:
		//-----------------------------------------------------------------------------------------

		public static implicit operator Vector4Int(Vector4IntRef aRef) {
			return aRef.Val;
		}
	}
}