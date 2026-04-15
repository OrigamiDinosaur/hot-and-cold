#if !UNITY_2017_2_OR_NEWER

using System;
using UnityEngine;

namespace Apache.Core {

	[Serializable]
	public struct Vector2Int {

		//-----------------------------------------------------------------------------------------
		// Constants:
		//-----------------------------------------------------------------------------------------

		// N.B. the below are named similar to Unity properties.
		// ReSharper disable InconsistentNaming

		public static readonly Vector2Int zero      = new Vector2Int( 0,  0);
		public static readonly Vector2Int one       = new Vector2Int( 1,  1);
		public static readonly Vector2Int negOne    = new Vector2Int(-1, -1);

		public static readonly Vector2Int up        = new Vector2Int( 0,  1);
		public static readonly Vector2Int right     = new Vector2Int( 1,  0);
		public static readonly Vector2Int down      = new Vector2Int( 0, -1);
		public static readonly Vector2Int left      = new Vector2Int(-1,  0);

		public static readonly Vector2Int upRight   = new Vector2Int( 1,  1);
		public static readonly Vector2Int downRight = new Vector2Int( 1, -1);
		public static readonly Vector2Int downLeft  = new Vector2Int(-1, -1);
		public static readonly Vector2Int upLeft    = new Vector2Int(-1,  1);

		public static readonly Vector2Int north     = new Vector2Int( 0,  1);
		public static readonly Vector2Int east      = new Vector2Int( 1,  0);
		public static readonly Vector2Int south     = new Vector2Int( 0, -1);
		public static readonly Vector2Int west      = new Vector2Int(-1,  0);

		public static readonly Vector2Int northEast = new Vector2Int( 1,  1);
		public static readonly Vector2Int southEast = new Vector2Int( 1, -1);
		public static readonly Vector2Int southWest = new Vector2Int(-1, -1);
		public static readonly Vector2Int northWest = new Vector2Int(-1,  1);

		public static readonly Vector2Int minValue  = new Vector2Int(int.MinValue, int.MinValue);
		public static readonly Vector2Int maxValue  = new Vector2Int(int.MaxValue, int.MaxValue);

		// ReSharper restore InconsistentNaming

		//-----------------------------------------------------------------------------------------
		// Private Fields:
		//-----------------------------------------------------------------------------------------

		private static Vector2Int[] cardinals;
		private static Vector2Int[] ordinals;
		private static Vector2Int[] cardinalsAndOrdinals;

		//-----------------------------------------------------------------------------------------
		// Public Fields:
		//-----------------------------------------------------------------------------------------

		// ReSharper disable InconsistentNaming
		public int x;
		public int y;
		// ReSharper restore InconsistentNaming

		//-----------------------------------------------------------------------------------------
		// Public Properties:
		//-----------------------------------------------------------------------------------------

		/// <summary>
		/// The North, South, East, and West cardinal directions. (Horizontal & Vertical/ Axials).
		/// </summary>
		public static Vector2Int[] Cardinals => cardinals ?? (cardinals = new [] { north, east, south, west });

		/// <summary>
		/// The NorthEast, SouthEast, SouthWest and NorthWest ordinal directions. (Diagonals)
		/// </summary>
		public static Vector2Int[] Ordinals => ordinals ?? (ordinals = new [] { northEast, southEast, southWest, northWest });

		/// <summary>
		/// All Cardiaonals and Ordinals.
		/// </summary>
		public static Vector2Int[] CardinalsAndOrdinals => cardinalsAndOrdinals ?? (cardinalsAndOrdinals = new [] { north, northEast, east, southEast, south, southWest, west, northWest });

		/// <summary>
		/// Returns the most major axis. If axis are equal in importance, y-axis takes precedence.
		/// </summary>
		public Vector2Int MajorAxis {
			get {
				if (x == 0 && y == 0) return zero;
				return Mathf.Abs(x) > Mathf.Abs(y) ? new Vector2Int(Math.Sign(x), 0) : new Vector2Int(0, Math.Sign(y));
			}
		}

		public int SqrMagnitude => x * x + y * y;

		public float Magnitude => Mathf.Sqrt(x * x + y * y);

		//-----------------------------------------------------------------------------------------
		// Constructors:
		//-----------------------------------------------------------------------------------------

		public Vector2Int(Vector2 v) {
			x = (int)v.x;
			y = (int)v.y;
		}

		public Vector2Int(float x, float y) {
			this.x = (int)x;
			this.y = (int)y;
		}

		public Vector2Int(int x, int y) {
			this.x = x;
			this.y = y;
		}

		//-----------------------------------------------------------------------------------------
		// Public Operators:
		//-----------------------------------------------------------------------------------------

		// Explicit cast to Vector2
		public static explicit operator Vector2(Vector2Int v) {
			return new Vector2(v.x, v.y);
		}

		// Explicit cast to Vector3
		public static explicit operator Vector3(Vector2Int v) {
			return new Vector3(v.x, v.y, 0);
		}

		// Addition
		public static Vector2Int operator +(Vector2Int a, Vector2Int b) {
			return new Vector2Int(a.x + b.x, a.y + b.y);
		}

		// Subtraction
		public static Vector2Int operator -(Vector2Int a, Vector2Int b) {
			return new Vector2Int(a.x - b.x, a.y - b.y);
		}

		// Multiplication
		public static Vector2Int operator *(Vector2Int a, int b) {
			return new Vector2Int(a.x * b, a.y * b);
		}

		// Int Multiplication
		public static Vector2Int operator *(int a, Vector2Int b) {
			return new Vector2Int(a * b.x, a * b.y);
		}

		// Division
		public static Vector2Int operator /(Vector2Int a, int b) {
			return new Vector2Int(a.x / b, a.y / b);
		}

		// Inversion
		public static Vector2Int operator -(Vector2Int a) {
			return new Vector2Int(-a.x, -a.y);
		}

		// Equality
		public static bool operator ==(Vector2Int a, Vector2Int b) {
			if (a.x == b.x) return a.y == b.y;
			return false;
		}

		// InEquality
		public static bool operator !=(Vector2Int a, Vector2Int b) {
			if (a.x == b.x) return a.y != b.y;
			return true;
		}

		//-----------------------------------------------------------------------------------------
		// Public Methods:
		//-----------------------------------------------------------------------------------------

		public Vector2 ToVector2() {
			return new Vector2(x, y);
		}

		public Vector3 ToVector3() {
			return new Vector3(x, y, 0.0f);
		}

		public bool IsWithin(Vector2Int min, Vector2Int max) {
			if (x >= min.x && x <= max.x && y >= min.y) return y <= max.y;
			return false;
		}

		public override string ToString() {
			return string.Format("{0},{1}", x, y);
		}

		public bool Equals(Vector2Int other) {
			return this == other;
		}

		public override bool Equals(object obj) {
			if (obj is Vector2Int) return this == (Vector2Int)obj;
			return base.Equals(obj);
		}

		public override int GetHashCode() {
			// N.B. We want the HashCode to change since this is a value type.
			// Using a variable HashCode allows quick HashCode comparisons between values to equate equality.
			// ReSharper disable NonReadonlyMemberInGetHashCode
			return 100267 * x + 200233 * y;
			// ReSharper restore NonReadonlyMemberInGetHashCode
		}

		public Vector2Int WithX(int newX) {
			return new Vector2Int(newX, y);
		}

		public Vector2Int WithY(int newY) {
			return new Vector2Int(x, newY);
		}

		//-----------------------------------------------------------------------------------------
		// Static Methods:
		//-----------------------------------------------------------------------------------------

		/// <summary>
		/// The Manhattan distance is the sum of the horizontal and vertical components.
		/// </summary>
		public static int ManhattanDistance(Vector2Int a, Vector2Int b) {
			return ManhattanDistance(a.x, a.y, b.x, b.y);
		}

		public static int ManhattanDistance(Vector2Int a, int bx, int by) {
			return ManhattanDistance(a.x, a.y, bx, by);
		}

		public static int ManhattanDistance(int ax, int ay, int bx, int by) {
			return Math.Abs(ax - bx) + Math.Abs(ay - by);
		}

		public static float Distance(Vector2Int a, Vector2Int b) {
			return Distance(a.x, a.y, b.x, b.y);
		}

		public static float Distance(Vector2Int a, int bx, int by) {
			return Distance(a.x, a.y, bx, by);
		}

		public static float Distance(int ax, int ay, int bx, int by) {
			return Mathf.Sqrt(DistanceSquared(ax, ay, bx, by));
		}

		public static float DistanceSquared(Vector2Int a, Vector2Int b) {
			return DistanceSquared(a.x, a.y, b.x, b.y);
		}

		public static float DistanceSquared(Vector2Int a, int bx, int by) {
			return DistanceSquared(a.x, a.y, bx, by);
		}

		public static float DistanceSquared(int ax, int ay, int bx, int by) {
			return (bx - ax) * (bx - ax) + (by - ay) * (by - ay);
		}

		public void Clamp(Vector2Int min, Vector2Int max) {
			x = Math.Max(x, min.x);
			y = Math.Max(y, min.y);
			x = Math.Min(x, max.x);
			y = Math.Min(y, max.y);
		}

		public static Vector2Int Scale(Vector2Int lhs, Vector2Int rhs) {
			return new Vector2Int(lhs.x * rhs.x, lhs.y * rhs.y);
		}

		public static Vector2Int Min(Vector2Int lhs, Vector2Int rhs) {
			return new Vector2Int(Mathf.Min(lhs.x, rhs.x), Mathf.Min(lhs.y, rhs.y));
		}

		public static Vector2Int Max(Vector2Int lhs, Vector2Int rhs) {
			return new Vector2Int(Mathf.Max(lhs.x, rhs.x), Mathf.Max(lhs.y, rhs.y));
		}
	}
}

#endif