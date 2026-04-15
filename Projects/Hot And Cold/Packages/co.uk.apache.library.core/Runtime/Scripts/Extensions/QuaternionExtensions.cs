using UnityEngine;

namespace Apache.Core.Extensions {
	public static class QuaternionExtensions {

		//-----------------------------------------------------------------------------------------
		// Public Methods:
		//-----------------------------------------------------------------------------------------

		/// <summary>Returns whether all components of this <c>Quaternion</c> are NaN.</summary>
		public static bool IsNaN(this Quaternion self) {
			return (float.IsNaN(self.x) && float.IsNaN(self.y) && float.IsNaN(self.z) && float.IsNaN(self.w));
		}

		/// <summary>Returns whether any components of this <c>Quaternion</c> is NaN.</summary>
		public static bool HasNaN(this Quaternion self) {
			return (float.IsNaN(self.x) || float.IsNaN(self.y) || float.IsNaN(self.z) || float.IsNaN(self.w));
		}

		/// <summary>Returns the forward <c>Vector3</c> of this <c>Quaternion</c>.</summary>
		public static Vector3 GetForward(this Quaternion self) {
			return self * Vector3.forward;
		}

		/// <summary>Returns the back <c>Vector3</c> of this <c>Quaternion</c>.</summary>
		public static Vector3 GetBack(this Quaternion self) {
			return self * Vector3.back;
		}

		/// <summary>Returns the up <c>Vector3</c> of this <c>Quaternion</c>.</summary>
		public static Vector3 GetUp(this Quaternion self) {
			return self * Vector3.up;
		}

		/// <summary>Returns the down <c>Vector3</c> of this <c>Quaternion</c>.</summary>
		public static Vector3 GetDown(this Quaternion self) {
			return self * Vector3.down;
		}

		/// <summary>Returns the left <c>Vector3</c> of this <c>Quaternion</c>.</summary>
		public static Vector3 GetLeft(this Quaternion self) {
			return self * Vector3.left;
		}

		/// <summary>Returns the right <c>Vector3</c> of this <c>Quaternion</c>.</summary>
		public static Vector3 GetRight(this Quaternion self) {
			return self * Vector3.right;
		}

		/// <summary>Returns the yaw of this <c>Quaternion</c> in degrees.</summary>
		public static float GetYaw(this Quaternion self) {
			return Mathf.Atan2(((self.y * self.w) + (self.x * self.z)) * 2, 1 - (self.y * self.y * 2) - (self.z * self.z * 2))
			     * Mathf.Rad2Deg;
		}

		/// <summary>Returns the pitch of this <c>Quaternion</c> in degrees.</summary>
		public static float GetPitch(this Quaternion self) {
			return Mathf.Atan2(((self.x * self.w) + (self.y * self.z)) * 2, 1 - (self.x * self.x * 2) - (self.z * self.z * 2))
			     * Mathf.Rad2Deg;
		}

		/// <summary>Returns the roll of this <c>Quaternion</c> in degrees.</summary>
		public static float GetRoll(this Quaternion self) {
			return Mathf.Atan2(((self.x * self.y) + (self.w * self.z)) * 2, (self.w * self.w) + (self.x * self.x) - (self.y * self.y) - (self.z * self.z))
			     * Mathf.Rad2Deg;
		}

		/// <summary>Returns a <c>Vector3</c> angular velocity representation of this <c>Quaternion</c>.</summary>
		public static Vector3 ToAngularVelocity(this Quaternion self) {

			// return a default vector if our magnitude overshoots.
			if (Mathf.Abs(self.w) > 1023.5f / 1024) return new Vector3();

			// work out gain which will be our components' magnitude.
			float angle = Mathf.Acos(Mathf.Abs(self.w));
			float gain  = Mathf.Sign(self.w) * 2 * (angle / Mathf.Sin(angle));

			return new Vector3(self.x * gain, self.y * gain, self.z * gain);
		}

		/// <summary>Returns a <c>Vector4</c> representation of this <c>Quaternion</c>.</summary>
		public static Vector4 ToVector4(this Quaternion self) {
			return new Vector4(self.x, self.y, self.z, self.w);
		}
	}
}