using System;
using UnityEngine;

namespace Apache.Core {

	[Serializable]
	public class Vector2IntRef : ValueRef<Vector2Int, Vector2IntContainer> {

		//-----------------------------------------------------------------------------------------
		// Operator Overrides:
		//-----------------------------------------------------------------------------------------

		public static implicit operator Vector2Int(Vector2IntRef aRef) {
			return aRef.Val;
		}
	}
}