using System;
using UnityEngine;

namespace Apache.Core {

	[Serializable]
	public class TextureRef : ValueRef<Texture, TextureContainer> {

		//-----------------------------------------------------------------------------------------
		// Operator Overrides:
		//-----------------------------------------------------------------------------------------

		public static implicit operator Texture(TextureRef aRef) {
			return aRef.Val;
		}
	}
}