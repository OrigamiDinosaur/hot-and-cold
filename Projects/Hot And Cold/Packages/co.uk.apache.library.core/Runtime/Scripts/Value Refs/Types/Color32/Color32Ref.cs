using System;
using UnityEngine;

namespace Apache.Core {

	[Serializable]
	public class Color32Ref : ValueRef<Color32, Color32Container> {

		//-----------------------------------------------------------------------------------------
		// Operator Overrides:
		//-----------------------------------------------------------------------------------------

		public static implicit operator Color32(Color32Ref aRef) {
			return aRef.Val;
		}
	}
}