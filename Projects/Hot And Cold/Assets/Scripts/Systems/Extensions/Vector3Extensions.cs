using UnityEngine;

public static class Vector3Extensions {

	//-----------------------------------------------------------------------------------------
	// Public Methods:
	//-----------------------------------------------------------------------------------------

	/// <summary>Returns whether all components of this <c>Vector3</c> are NaN.</summary>
	public static bool IsNaN(this Vector3 self) {
		return (float.IsNaN(self.x) && float.IsNaN(self.y) && float.IsNaN(self.z));
	}

	/// <summary>Returns whether any components of this <c>Vector3</c> is NaN.</summary>
	public static bool HasNaN(this Vector3 self) {
		return (float.IsNaN(self.x) || float.IsNaN(self.y) || float.IsNaN(self.z));
	}

	/// <summary>Returns a version of this <c>Vector2</c> with absolute <c>x</c>, <c>y</c>, and <c>z</c> components.</summary>
	public static Vector3 Abs(this Vector3 self) {
		self.x = Mathf.Abs(self.x);
		self.y = Mathf.Abs(self.y);
		self.z = Mathf.Abs(self.z);
		return self;
	}

	/// <summary>Returns the angle of this <c>Vector3</c>s <c>x</c> and <c>y</c> components in radians.</summary>
	public static float AngleRad(this Vector3 self) {
		return Mathf.Atan2(self.x, self.y);
	}

	/// <summary>Returns the angle of this <c>Vector3</c>s <c>x</c> and <c>y</c> components in degrees.</summary>
	public static float AngleDeg(this Vector3 self) {
		return AngleRad(self) * Mathf.Rad2Deg;
	}

	/// <summary>Returns whether this <c>Vector3</c> is approximately equal to <c>other</c>.</summary>
	public static bool Approximately(this Vector3 self, Vector3 other) {
		return (Mathf.Approximately(self.x, other.x) && Mathf.Approximately(self.y, other.y) && Mathf.Approximately(self.z, other.z));
	}

	/// <summary>
	/// Returns a <c>Vector3</c> with this <c>Vector3</c>'s components clamped within the range <c>(min.x, min.y, min.z)</c> to
	/// <c>(max.x, max.y, max.z)</c>.
	/// </summary>
	public static Vector3 Clamp(this Vector3 self, Vector3 min, Vector3 max) {
		return new Vector3(Mathf.Clamp(self.x, min.x, max.x), Mathf.Clamp(self.y, min.y, max.y), Mathf.Clamp(self.z, min.z, max.z));
	}

	/// <summary>
	/// Returns a <c>Vector3</c> with this <c>Vector3</c>'s components clamped within the range <c>(0, 0, 0)</c> to <c>(1, 1, 1)</c>.
	/// </summary>
	public static Vector3 Clamp01(this Vector3 self) {
		return self.Clamp(Vector3.zero, Vector3.one);
	}

	/// <summary>
	/// Returns a <c>Vector3</c> with this <c>Vector3</c>'s components clamped within the range <c>(-1, -1, -1)</c> to <c>(1, 1, 1)</c>.
	/// </summary>
	public static Vector3 ClampMinus1To1(this Vector3 self) {
		return self.Clamp(Vector3.zero, Vector3.one);
	}

	/// <summary>
	/// Returns a <c>Vector3</c> with this <c>Vector3</c>'s <c>x</c>, <c>y</c>, and <c>z</c> components raised to the given power
	/// <c>p</c>.
	/// </summary>
	public static Vector3 Pow(this Vector3 self, float p) {
		self.x = Mathf.Pow(self.x, p);
		self.y = Mathf.Pow(self.y, p);
		self.z = Mathf.Pow(self.z, p);
		return self;
	}

	/// <summary>
	/// Returns whether the <c>x</c>, <c>y</c>, and <c>z</c> components of this <c>Vector3</c> are within a zero to one range.
	/// </summary>
	public static bool IsWithin0To1(this Vector3 self) {
		return (self.x >= 0 && self.x <= 1 && self.y >= 0 && self.y <= 1 && self.z >= 0 && self.z <= 1);
	}

	/// <summary>
	/// Returns whether the <c>x</c>, <c>y</c>, and <c>z</c> components of this <c>Vector3</c> are within a minus one to one range.
	/// </summary>
	public static bool IsWithinMinus1To1(this Vector3 self) {
		return (self.x >= -1 && self.x <= 1 && self.y >= -1 && self.y <= 1 && self.z >= -1 && self.z <= 1);
	}

	/// <summary>
	/// Returns whether the <c>x</c>, <c>y</c>, and <c>z</c> components of this <c>Vector3</c> are within the range denoted by the given
	/// <c>min</c> and <c>max</c> <c>Vector2</c>s.
	/// </summary>
	public static bool IsWithin(this Vector3 self, Vector3 min, Vector3 max) {
		return (self.x >= min.x && self.x <= max.x && self.y >= min.y && self.y <= max.y && self.z >= min.z && self.z <= max.z);
	}

	/// <summary>Returns this <c>Vector3</c> with the given <c>x</c> component.</summary>
	public static Vector3 WithX(this Vector3 self, float x) {
		return new Vector3(x, self.y, self.z);
	}

	/// <summary>Returns this <c>Vector3</c> with the given <c>y</c> component.</summary>
	public static Vector3 WithY(this Vector3 self, float y) {
		return new Vector3(self.x, y, self.z);
	}

	/// <summary>Returns this <c>Vector3</c> with the given <c>z</c> component.</summary>
	public static Vector3 WithZ(this Vector3 self, float z) {
		return new Vector3(self.x, self.y, z);
	}

	/// <summary>Returns a <c>Vector2Int</c> with this <c>Vector2</c>'s <c>x</c> and <c>y</c> components casted to <c>int</c>s.</summary>
	public static Vector2Int ToVector2Int(this Vector3 self) {
		return new Vector2Int((int)self.x, (int)self.y);
	}

	/// <summary>Returns a <c>Vector2Int</c> with this <c>Vector3</c>'s components casted to <c>int</c>s.</summary>
	public static Vector3Int ToVector3Int(this Vector3 self) {
		return new Vector3Int((int)self.x, (int)self.y, (int)self.z);
	}

	/// <summary>Returns a <c>Quaternion</c> from this angular velocity <c>Vector3</c>.</summary>
	public static Quaternion FromAngularVelocity(this Vector3 self) {

		// grab magnitude and return identity if there is none.
		float magnitude = self.magnitude;
		if (magnitude <= 0) return Quaternion.identity;

		// work out sin gain.
		float cos = Mathf.Cos(magnitude * 0.5f);
		float sinGain = Mathf.Sin(magnitude * 0.5f) / magnitude;

		return new Quaternion(self.x * sinGain, self.y * sinGain, self.z * sinGain, cos);
	}
}
