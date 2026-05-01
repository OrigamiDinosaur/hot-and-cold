using UnityEngine;

public static class Vector2Extensions {

	//-----------------------------------------------------------------------------------------
	// Public Methods:
	//-----------------------------------------------------------------------------------------

	/// <summary>Returns whether all components of this <c>Vector2</c> are NaN.</summary>
	public static bool IsNaN(this Vector2 self) {
		return (float.IsNaN(self.x) && float.IsNaN(self.y));
	}

	/// <summary>Returns whether any components of this <c>Vector2</c> is NaN.</summary>
	public static bool HasNaN(this Vector2 self) {
		return (float.IsNaN(self.x) || float.IsNaN(self.y));
	}

	/// <summary>Returns a version of this <c>Vector2</c> with absolute <c>x</c> and <c>y</c> components.</summary>
	public static Vector2 Abs(this Vector2 self) {
		self.x = Mathf.Abs(self.x);
		self.y = Mathf.Abs(self.y);
		return self;
	}

	/// <summary>Returns the angle of this <c>Vector2</c> in radians.</summary>
	public static float AngleRad(this Vector2 self) {
		return Mathf.Atan2(self.x, self.y);
	}

	/// <summary>Returns the angle of this <c>Vector2</c> in degrees.</summary>
	public static float AngleDeg(this Vector2 self) {
		return AngleRad(self) * Mathf.Rad2Deg;
	}

	/// <summary>Returns whether this <c>Vector2</c> is approximately equal to <c>other</c>.</summary>
	public static bool Approximately(this Vector2 self, Vector2 other) {
		return (Mathf.Approximately(self.x, other.x) && Mathf.Approximately(self.y, other.y));
	}

	/// <summary>
	/// Returns a <c>Vector2</c> with this <c>Vector2</c>'s <c>x</c> and <c>y</c> components clamped within the range <c>(min.x, min.y)</c>
	/// to <c>(max.x, max.y)</c>.
	/// </summary>
	public static Vector2 Clamp(this Vector2 self, Vector2 min, Vector2 max) {
		self.x = Mathf.Clamp(self.x, min.x, max.x);
		self.y = Mathf.Clamp(self.y, min.y, max.y);
		return self;
	}

	/// <summary>
	/// Returns a <c>Vector2</c> with this <c>Vector2</c>'s <c>x</c> and <c>y</c> components clamped within the range <c>(0, 0)</c> to
	/// <c>(1, 1)</c>.
	/// </summary>
	public static Vector2 Clamp01(this Vector2 self) {
		return self.Clamp(Vector2.zero, Vector2.one);
	}

	/// <summary>
	/// Returns a <c>Vector2</c> with this <c>Vector2</c>'s <c>x</c> and <c>y</c> components clamped within the range <c>(-1, -1)</c> to
	/// <c>(1, 1)</c>.
	/// </summary>
	public static Vector2 ClampMinus1To1(this Vector2 self) {
		return self.Clamp(-Vector2.one, Vector2.one);
	}

	/// <summary>Returns a <c>Vector2</c> with this <c>Vector2</c>'s <c>x</c> and <c>y</c> components raised to the given power <c>p</c>.</summary>
	public static Vector2 Pow(this Vector2 self, float p) {
		self.x = Mathf.Pow(self.x, p);
		self.y = Mathf.Pow(self.y, p);
		return self;
	}
	
	/// <summary>Returns whether both <c>x</c> and <c>y</c> components of this <c>Vector2</c> are within the zero to one range.</summary>
	public static bool IsWithin0To1(this Vector2 self) {
		return (self.x >= 0 && self.x <= 1 && self.y >= 0 && self.y <= 1);
	}

	/// <summary>Returns whether both <c>x</c> and <c>y</c> components of this <c>Vector2</c> are within the minus one to one range.</summary>
	public static bool IsWithinMinus1To1(this Vector2 self) {
		return (self.x >= -1 && self.x <= 1 && self.y >= -1 && self.y <= 1);
	}

	/// <summary>
	/// Returns whether both <c>x</c> and <c>y</c> components of this <c>Vector2</c> are within the range denoted by the given <c>min</c>
	/// and <c>max</c> <c>Vector2</c>s.
	/// </summary>
	public static bool IsWithin(this Vector2 self, Vector2 min, Vector2 max) {
		return (self.x >= min.x && self.x <= max.x && self.y >= min.y && self.y <= max.y);
	}

	/// <summary>Returns this <c>Vector2</c> with the given <c>x</c> component.</summary>
	public static Vector2 WithX(this Vector2 self, float x) {
		return new Vector2(x, self.y);
	}

	/// <summary>Returns this <c>Vector2</c> with the given <c>y</c> component.</summary>
	public static Vector2 WithY(this Vector2 self, float y) {
		return new Vector2(self.x, y);
	}

	/// <summary>Returns a <c>Vector2Int</c> with this <c>Vector2</c>'s components casted to <c>int</c>s.</summary>
	public static Vector2Int ToVector2Int(this Vector2 self) {
		return new Vector2Int((int)self.x, (int)self.y);
	}

	/// <summary>Returns a <c>Vector3Int</c> with this <c>Vector2</c>'s components casted to <c>int</c>s and a value of zero for <c>z</c>.</summary>
	public static Vector3Int ToVector3Int(this Vector2 self) {
		return new Vector3Int((int)self.x, (int)self.y, 0);
	}
}