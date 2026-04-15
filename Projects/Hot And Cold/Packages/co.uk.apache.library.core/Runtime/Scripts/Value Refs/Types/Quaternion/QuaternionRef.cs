using System;
using UnityEngine;

namespace Apache.Core {

	[Serializable]
	public class QuaternionRef : ValueRef<Quaternion, QuaternionContainer> {

		//-----------------------------------------------------------------------------------------
		// Operator Overrides:
		//-----------------------------------------------------------------------------------------

		public static implicit operator Quaternion(QuaternionRef aRef) {
			return aRef.Val;
		}
	}
}