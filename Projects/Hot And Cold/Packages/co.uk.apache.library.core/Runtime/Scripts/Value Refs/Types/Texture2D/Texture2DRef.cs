using System;
using UnityEngine;

namespace Apache.Core {

	[Serializable]
	public class Texture2DRef : ValueRef<Texture2D, Texture2DContainer> {

		//-----------------------------------------------------------------------------------------
		// Operator Overrides:
		//-----------------------------------------------------------------------------------------

		public static implicit operator Texture2D(Texture2DRef aRef) {
			return aRef.Val;
		}
	}
}