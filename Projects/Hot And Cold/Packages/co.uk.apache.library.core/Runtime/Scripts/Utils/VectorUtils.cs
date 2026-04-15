using UnityEngine;

namespace Apache.Core {
	public static class VectorUtils {

		//-----------------------------------------------------------------------------------------
		// Public Fields:
		//-----------------------------------------------------------------------------------------

		/// <summary>
		/// A <c>Vector2</c> comprised of <c>float.NaN</c> components.
		/// </summary>
		public static readonly Vector2 NaNVector2 = new Vector2(float.NaN, float.NaN);

		/// <summary>
		/// A <c>Vector3</c> comprised of <c>float.NaN</c> components.
		/// </summary>
		public static readonly Vector3 NaNVector3 = new Vector3(float.NaN, float.NaN, float.NaN);

		//-----------------------------------------------------------------------------------------
		// Public Methods:
		//-----------------------------------------------------------------------------------------

		/// <summary>
		/// Returns a <c>Vector2</c> whose x and y coordinates are the minimum values of the given two <c>Vector2</c>s.
		/// </summary>
		public static Vector2 Min(Vector2 a, Vector2 b) {
			return new Vector2(Mathf.Min(a.x, b.x), Mathf.Min(a.y, b.y));
		}

		/// <summary>
		/// Returns a <c>Vector2</c> whose x and y coordinates are the maximum values of the given two <c>Vector2</c>s.
		/// </summary>
		public static Vector2 Max(Vector2 a, Vector2 b) {
			return new Vector2(Mathf.Max(a.x, b.x), Mathf.Max(a.y, b.y));
		}

		/// <summary>
		/// Returns a <c>Vector2Int</c> whose x and y coordinates are the minimum values of the given two <c>Vector2Int</c>s.
		/// </summary>
		public static Vector2Int Min(Vector2Int a, Vector2Int b) {
			return new Vector2Int(Mathf.Min(a.x, b.x), Mathf.Min(a.y, b.y));
		}

		/// <summary>
		/// Returns a <c>Vector2Int</c> whose x and y coordinates are the maximum values of the given two <c>Vector2Int</c>s.
		/// </summary>
		public static Vector2Int Max(Vector2Int a, Vector2Int b) {
			return new Vector2Int(Mathf.Max(a.x, b.x), Mathf.Max(a.y, b.y));
		}

		/// <summary>
		/// Interpolates between the given <c>Vector2</c>s to <c>t</c>, with smoothing at the limits.
		/// </summary>
		public static Vector2 SmoothStep(Vector2 a, Vector2 b, float t) {
			return new Vector2(Mathf.SmoothStep(a.x, b.x, t), Mathf.SmoothStep(a.y, b.y, t));
		}
	}
}