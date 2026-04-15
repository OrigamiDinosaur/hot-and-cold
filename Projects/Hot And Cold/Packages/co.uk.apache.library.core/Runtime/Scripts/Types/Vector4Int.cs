using System;
using UnityEngine;

namespace Apache.Core {

	[Serializable]
	public struct Vector4Int {

		//-----------------------------------------------------------------------------------------
		// Constants:
		//-----------------------------------------------------------------------------------------

		public static readonly Vector4Int Zero   = new Vector4Int( 0,  0,  0,  0);
		public static readonly Vector4Int One    = new Vector4Int( 1,  1,  1,  1);
		public static readonly Vector4Int NegOne = new Vector4Int(-1, -1, -1, -1);

		public static readonly Vector4Int MinValue = new Vector4Int(int.MinValue, int.MinValue, int.MinValue, int.MinValue);
		public static readonly Vector4Int MaxValue = new Vector4Int(int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue);

		//-----------------------------------------------------------------------------------------
		// Public Fields:
		//-----------------------------------------------------------------------------------------

		// ReSharper disable InconsistentNaming

		public int x;
		public int y;
		public int z;
		public int w;

		// ReSharper restore InconsistentNaming

		//-----------------------------------------------------------------------------------------
		// Public Properties:
		//-----------------------------------------------------------------------------------------

		public int SqrMagnitude => x * x + y * y + z * z + w * w;

		public float Magnitude => Mathf.Sqrt(SqrMagnitude);

		//-----------------------------------------------------------------------------------------
		// Constructors:
		//-----------------------------------------------------------------------------------------

		public Vector4Int(float x, float y, float z, float w) {
			this.x = (int) x;
			this.y = (int) y;
			this.z = (int) z;
			this.w = (int) w;
		}

		public Vector4Int(int x, int y, int z, int w) {
			this.x = x;
			this.y = y;
			this.z = z;
			this.w = w;
		}

		//-----------------------------------------------------------------------------------------
		// Public Operators:
		//-----------------------------------------------------------------------------------------

		// Explicit cast to Vector2.
		public static explicit operator Vector4(Vector4Int v) {
			return new Vector4(v.x, v.y, v.z, v.w);
		}

		// Addition.
		public static Vector4Int operator +(Vector4Int a, Vector4Int b) {
			return new Vector4Int(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);
		}

		// Subtraction.
		public static Vector4Int operator -(Vector4Int a, Vector4Int b) {
			return new Vector4Int(a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w);
		}

		// Multiplication.
		public static Vector4Int operator *(Vector4Int a, int b) {
			return new Vector4Int(a.x * b, a.y * b, a.z * b, a.w * b);
		}

		// Int multiplication.
		public static Vector4Int operator *(int a, Vector4Int b) {
			return new Vector4Int(a * b.x, a * b.y, a * b.z, a * b.w);
		}

		// Division.
		public static Vector4Int operator /(Vector4Int a, int b) {
			return new Vector4Int(a.x / b, a.y / b, a.z / b, a.w / b);
		}

		// Inversion.
		public static Vector4Int operator -(Vector4Int a) {
			return new Vector4Int(-a.x, -a.y, -a.z, -a.w);
		}

		// Equality.
		public static bool operator ==(Vector4Int a, Vector4Int b) {
			return (a.x == b.x && a.y == b.y && a.z == b.z && a.w == b.w);
		}

		// Inequality.
		public static bool operator !=(Vector4Int a, Vector4Int b) {
			return (a.x != b.x && a.y != b.y && a.z != b.z && a.w != b.w);
		}

		//-----------------------------------------------------------------------------------------
		// Public Methods:
		//-----------------------------------------------------------------------------------------

		public Vector4 ToVector4() {
			return new Vector4(x, y, z, w);
		}

		public bool IsWithin(Vector4Int min, Vector4Int max) {
			return (x >= min.x && x <= max.x && y >= min.y && y <= max.y && z >= min.z && z <= max.z && w >= min.w && w <= max.w);
		}

		public override string ToString() {
			return $"{x},{y},{z},{w}";
		}

		public bool Equals(Vector4Int other) {
			return this == other;
		}

		public override bool Equals(object obj) {
			// ReSharper disable once MergeCastWithTypeCheck
			if (obj is Vector4Int) return (this == (Vector4Int)obj);
			return base.Equals(obj);
		}

		public override int GetHashCode() {

			// N.B. We want the HashCode to change since this is a value type.
			// Using a variable HashCode allows quick HashCode comparisons between values to equate equality.

			// ReSharper disable NonReadonlyMemberInGetHashCode
			return 10267 * x + 20033 * y + 400992 * z + 812921 * w;
			// ReSharper restore NonReadonlyMemberInGetHashCode
		}

		public Vector4Int WithX(int newX) {
			return new Vector4Int(newX, y, z, w);
		}

		public Vector4Int WithY(int newY) {
			return new Vector4Int(x, newY, z, w);
		}

		public Vector4Int WithZ(int newZ) {
			return new Vector4Int(x, y, newZ, w);
		}

		public Vector4Int WithW(int newW) {
			return new Vector4Int(x, y, z, newW);
		}

		public void Clamp(Vector4Int min, Vector4Int max) {
			x = Math.Max(x, min.x);
			y = Math.Max(y, min.y);
			z = Math.Max(z, max.z);
			w = Math.Max(w, max.w);

			x = Math.Min(x, max.x);
			y = Math.Min(y, max.y);
			z = Math.Min(z, max.z);
			w = Math.Min(w, max.w);
		}
	}
}