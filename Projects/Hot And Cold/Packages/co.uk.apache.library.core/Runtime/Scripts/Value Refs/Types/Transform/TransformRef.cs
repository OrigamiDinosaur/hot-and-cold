using System;
using UnityEngine;

namespace Apache.Core {

	[Serializable]
	public class TransformRef : ValueRef<Transform, TransformContainer> {

		//-----------------------------------------------------------------------------------------
		// Operator Overrides:
		//-----------------------------------------------------------------------------------------

		public static implicit operator Transform(TransformRef aRef) {
			return aRef.Val;
		}
	}
}