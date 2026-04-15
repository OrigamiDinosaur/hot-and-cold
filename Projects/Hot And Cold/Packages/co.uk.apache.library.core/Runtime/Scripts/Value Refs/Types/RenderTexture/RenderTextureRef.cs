using System;
using UnityEngine;

namespace Apache.Core {

	[Serializable]
	public class RenderTextureRef : ValueRef<RenderTexture, RenderTextureContainer> {

		//-----------------------------------------------------------------------------------------
		// Operator Overrides:
		//-----------------------------------------------------------------------------------------

		public static implicit operator RenderTexture(RenderTextureRef aRef) {
			return aRef.Val;
		}
	}
}