using UnityEngine;

namespace Apache.Core.Extensions {
	public static class Vector4Extensions {

		//-----------------------------------------------------------------------------------------
		// Public Methods:
		//-----------------------------------------------------------------------------------------

		/// <summary>Returns a <c>Quaternion</c> representation of this <c>Vector4</c>.</summary>
		public static Quaternion ToQuaternion(this Vector4 self) {
			return new Quaternion(self.x, self.y, self.z, self.w);
		}
	}
}