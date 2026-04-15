using System;
using UnityEngine;

namespace Apache.Core {

	[Serializable]
	public class Vector2Ref : ValueRef<Vector2, Vector2Container> {

		//-----------------------------------------------------------------------------------------
		// Operator Overrides:
		//-----------------------------------------------------------------------------------------

		public static implicit operator Vector2(Vector2Ref aRef) {
			return aRef.Val;
		}
	}
}