using System;
using UnityEngine;

namespace Apache.Core {

	[Serializable]
	public class Vector4Ref : ValueRef<Vector4, Vector4Container> {

		//-----------------------------------------------------------------------------------------
		// Operator Overrides:
		//-----------------------------------------------------------------------------------------

		public static implicit operator Vector4(Vector4Ref aRef) {
			return aRef.Val;
		}
	}
}