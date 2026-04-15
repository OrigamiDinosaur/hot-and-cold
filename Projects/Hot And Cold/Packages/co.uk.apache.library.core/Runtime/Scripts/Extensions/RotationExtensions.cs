using UnityEngine;

namespace Apache.Core.Extensions {
	public static class RotationExtensions {

		//-----------------------------------------------------------------------------------------
		// Public Methods:
		//-----------------------------------------------------------------------------------------

		public static Vector3 ToPitchYawRoll(this Quaternion self) {
			Vector3 rad = ToPitchYawRollRad(self);
			return new Vector3(rad.x * Mathf.Rad2Deg, rad.y * Mathf.Rad2Deg, rad.z * Mathf.Rad2Deg);
		}

		public static Vector3 ToPitchYawRollRad(this Quaternion self) {
			float pitch = Mathf.Atan2((2 * self.x * self.w) - (2 * self.y * self.z), 1 - (2 * self.x * self.x) - (2 * self.z * self.z));
			float yaw   = Mathf.Atan2((2 * self.y * self.w) - (2 * self.x * self.z), 1 - (2 * self.y * self.y) - (2 * self.z * self.z));
			float roll  = Mathf.Asin ((2 * self.x * self.y) + (2 * self.z * self.w));
			return new Vector3(pitch, yaw, roll);
		}

		public static Quaternion PitchOnly(this Quaternion self) {
			return Quaternion.Euler(self.ToPitchYawRoll().x, 0, 0);
		}

		public static Quaternion YawOnly(this Quaternion self) {
			return Quaternion.Euler(0, self.ToPitchYawRoll().y, 0);
		}

		public static Quaternion RollOnly(this Quaternion self) {
			return Quaternion.Euler(0, 0, self.ToPitchYawRoll().z);
		}
	}
}