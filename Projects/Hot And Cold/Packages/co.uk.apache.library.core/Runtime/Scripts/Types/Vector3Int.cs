#if !UNITY_2017_2_OR_NEWER

using System;
using UnityEngine;

namespace Apache.Core {

	[Serializable]
	public struct Vector3Int {

		//-----------------------------------------------------------------------------------------
		// Constants:
		//-----------------------------------------------------------------------------------------

		// N.B. the below are named similar to Unity properties.
		// ReSharper disable InconsistentNaming

		public static readonly Vector3Int zero     = new Vector3Int( 0,  0,  0);
		public static readonly Vector3Int one      = new Vector3Int( 1,  1,  1);
		public static readonly Vector3Int negOne   = new Vector3Int(-1, -1, -1);

		public static readonly Vector3Int up       = new Vector3Int( 0,  1,  0);
		public static readonly Vector3Int left     = new Vector3Int(-1,  0,  0);
		public static readonly Vector3Int down     = new Vector3Int( 0, -1,  0);
		public static readonly Vector3Int right    = new Vector3Int( 1,  0,  0);
		public static readonly Vector3Int forward  = new Vector3Int( 0,  0,  1);
		public static readonly Vector3Int back     = new Vector3Int( 0,  0, -1);

		public static readonly Vector3Int minValue = new Vector3Int(int.MinValue, int.MinValue, int.MinValue);
		public static readonly Vector3Int maxValue = new Vector3Int(int.MaxValue, int.MaxValue, int.MaxValue);

		// ReSharper restore InconsistentNaming

		//-----------------------------------------------------------------------------------------
		// Private Fields:
		//-----------------------------------------------------------------------------------------

		private static Vector3Int[] cardinals;

		//-----------------------------------------------------------------------------------------
		// Public Fields:
		//-----------------------------------------------------------------------------------------

		// ReSharper disable InconsistentNaming
		public int x;
		public int y;
		public int z;
		// ReSharper restore InconsistentNaming

		//-----------------------------------------------------------------------------------------
		// Public Properties:
		//-----------------------------------------------------------------------------------------

		/// <summary>
		/// The North, South, East, and West cardinal directions. (Horizontal & Vertical/ Axials).
		/// </summary>
		public static Vector3Int[] Cardinals => cardinals ?? (cardinals = new [] { up, down, left, right, forward, back });

		public int SqrMagnitude => x * x + y * y + z * z;

		public float Magnitude => Mathf.Sqrt(SqrMagnitude);

		//-----------------------------------------------------------------------------------------
		// Constructors:
		//-----------------------------------------------------------------------------------------

		public Vector3Int(Vector2 v) {
			x = (int)v.x;
			y = (int)v.y;
			z = 0;
		}

		public Vector3Int(float x, float y, float z) {
			this.x = (int)x;
			this.y = (int)y;
			this.z = (int)z;
		}

		public Vector3Int(int x, int y, int z) {
			this.x = x;
			this.y = y;
			this.z = z;
		}

		//-----------------------------------------------------------------------------------------
		// Public Operators:
		//-----------------------------------------------------------------------------------------

		// Explicit cast to Vector2
		public static explicit operator Vector3(Vector3Int v) {
			return new Vector3(v.x, v.y, v.z);
		}

		// Addition
		public static Vector3Int operator +(Vector3Int a, Vector3Int b) {
			return new Vector3Int(a.x + b.x, a.y + b.y, a.z + b.z);
		}

		// Subtraction
		public static Vector3Int operator -(Vector3Int a, Vector3Int b) {
			return new Vector3Int(a.x - b.x, a.y - b.y, a.z - b.z);
		}

		// Multiplication
		public static Vector3Int operator *(Vector3Int a, int b) {
			return new Vector3Int(a.x * b, a.y * b, a.z * b);
		}

		// Int Multiplication
		public static Vector3Int operator *(int a, Vector3Int b) {
			return new Vector3Int(a * b.x, a * b.y, a * b.z);
		}

		// Division
		public static Vector3Int operator /(Vector3Int a, int b) {
			return new Vector3Int(a.x / b, a.y / b, a.z / b);
		}

		// Inversion
		public static Vector3Int operator -(Vector3Int a) {
			return new Vector3Int(-a.x, -a.y, -a.z);
		}

		// Equality
		public static bool operator ==(Vector3Int a, Vector3Int b) {
			return (a.x == b.x && a.y == b.y && a.z == b.z);
		}

		// InEquality
		public static bool operator !=(Vector3Int a, Vector3Int b) {
			return (a.x != b.x && a.y != b.y && a.z != b.z);
		}

		//-----------------------------------------------------------------------------------------
		// Public Methods:
		//-----------------------------------------------------------------------------------------

		public Vector3 ToVector3() {
			return new Vector3(x, y, z);
		}

		public bool IsWithin(Vector3Int min, Vector3Int max) {
			return (x >= min.x && x <= max.x && y >= min.y && y <= max.y && z >= min.z && z <= max.z);
		}

		public override string ToString() {
			return string.Format("{0},{1},{2}", x, y, z);
		}

		public bool Equals(Vector3Int other) {
			return this == other;
		}

		public override bool Equals(object obj) {
			if (obj is Vector3Int) return this == (Vector3Int)obj;
			return base.Equals(obj);
		}

		public override int GetHashCode() {
			// See Vector2Int.GetHashCode.
			// ReSharper disable NonReadonlyMemberInGetHashCode
			return 100267 * x + 200233 * y + 400992 * z;
			// ReSharper restore NonReadonlyMemberInGetHashCode
		}

		public Vector3Int WithX(int newX) {
			return new Vector3Int(newX, y, z);
		}

		public Vector3Int WithY(int newY) {
			return new Vector3Int(x, newY, z);
		}

		public Vector3Int WithZ(int newZ) {
			return new Vector3Int(x, y, newZ);
		}

		//-----------------------------------------------------------------------------------------
		// Static Methods:
		//-----------------------------------------------------------------------------------------

		/// <summary>
		/// The Manhattan distance is the sum of the horizontal and vertical components.
		/// </summary>
		public static int ManhattanDistance(Vector3Int a, Vector3Int b) {
			return ManhattanDistance(a.x, a.y, a.z, b.x, b.y, b.z);
		}

		public static int ManhattanDistance(Vector3Int a, int bx, int by, int bz) {
			return ManhattanDistance(a.x, a.y, a.z, bx, by, bz);
		}

		public static int ManhattanDistance(int ax, int ay, int az, int bx, int by, int bz) {
			return Math.Abs(ax - bx) + Math.Abs(ay - by) + Math.Abs(az - bz);
		}

		public static float Distance(Vector3Int a, Vector3Int b) {
			return Mathf.Sqrt(DistanceSquared(a, b));
		}

		public static float Distance(Vector3Int a, int bx, int by, int bz) {
			return Mathf.Sqrt(DistanceSquared(a, bx, by, bz));
		}

		public static float Distance(int ax, int ay, int az, int bx, int by, int bz) {
			return Mathf.Sqrt(DistanceSquared(ax, bx, az, bx, by, bz));
		}

		public static float DistanceSquared(Vector3Int a, Vector3Int b) {
			return DistanceSquared(a.x, a.y, a.z, b.x, b.y, b.z);
		}

		public static float DistanceSquared(Vector3Int a, int bx, int by, int bz) {
			return DistanceSquared(a.x, a.y, a.z, bx, by, bz);
		}

		public static float DistanceSquared(int ax, int ay, int az, int bx, int by, int bz) {
			return (bx - ax) * (bx - ax) + (by - ay) * (by - ay) + (bz - az) * (bz - az);
		}

		public void Clamp(Vector3Int min, Vector3Int max) {
			x = Math.Max(x, min.x);
			y = Math.Max(y, min.y);
			x = Math.Min(x, max.x);
			y = Math.Min(y, max.y);
		}

		public static Vector3Int Scale(Vector3Int lhs, Vector3Int rhs) {
			return new Vector3Int(lhs.x * rhs.x, lhs.y * rhs.y, lhs.z * rhs.z);
		}

		public static Vector3Int Min(Vector3Int lhs, Vector3Int rhs) {
			return new Vector3Int(Mathf.Min(lhs.x, rhs.x), Mathf.Min(lhs.y, rhs.y), Mathf.Min(lhs.z, rhs.z));
		}

		public static Vector3Int Max(Vector3Int lhs, Vector3Int rhs) {
			return new Vector3Int(Mathf.Max(lhs.x, rhs.x), Mathf.Max(lhs.y, rhs.y), Mathf.Max(lhs.z, rhs.z));
		}
	}
}

#endif