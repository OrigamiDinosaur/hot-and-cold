using System;
using UnityEngine;

namespace Apache.Core {

	[Serializable]
	public class Vector3Ref : ValueRef<Vector3, Vector3Container> {

		//-----------------------------------------------------------------------------------------
		// Operator Overrides:
		//-----------------------------------------------------------------------------------------

		public static implicit operator Vector3(Vector3Ref aRef) {
			return aRef.Val;
		}
	}
}