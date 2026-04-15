using System;
using UnityEngine;

namespace Apache.Core {

	[Serializable]
	public class Vector3IntRef : ValueRef<Vector3Int, Vector3IntContainer> {

		//-----------------------------------------------------------------------------------------
		// Operator Overrides:
		//-----------------------------------------------------------------------------------------

		public static implicit operator Vector3Int(Vector3IntRef aRef) {
			return aRef.Val;
		}
	}
}