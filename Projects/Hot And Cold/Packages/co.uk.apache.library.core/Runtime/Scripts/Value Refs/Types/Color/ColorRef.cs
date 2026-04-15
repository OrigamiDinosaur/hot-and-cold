using System;
using UnityEngine;

namespace Apache.Core {

	[Serializable]
	public class ColorRef : ValueRef<Color, ColorContainer> {

		//-----------------------------------------------------------------------------------------
		// Operator Overrides:
		//-----------------------------------------------------------------------------------------

		public static implicit operator Color(ColorRef aRef) {
			return aRef.Val;
		}
	}
}