using System;
using UnityEngine;

namespace Apache.Core {

	[Serializable]
	public struct FloatRange {

		//-----------------------------------------------------------------------------------------
		// Inspector Variables:
		//-----------------------------------------------------------------------------------------

		[SerializeField] private float min;
		[SerializeField] private float max;

		//-----------------------------------------------------------------------------------------
		// Public Properties:
		//-----------------------------------------------------------------------------------------

		// ReSharper disable ArrangeAccessorOwnerBody

		public float Min {
			get { return min; }
			set { min = value; }
		}

		public float Max {
			get { return max; }
			set { max = value; }
		}

		// ReSharper restore ArrangeAccessorOwnerBody

		public float Difference => (max - min);

		//-----------------------------------------------------------------------------------------
		// Constructors:
		//-----------------------------------------------------------------------------------------

		public FloatRange(float min, float max) {
			this.min = min;
			this.max = max;
		}

		//-----------------------------------------------------------------------------------------
		// Implicit Operators:
		//-----------------------------------------------------------------------------------------

		public static implicit operator FloatRange(Vector2 v) {
			return new FloatRange(v.x, v.y);
		}

		public static implicit operator FloatRange(Vector3 v) {
			return new FloatRange(v.x, v.y);
		}

		public static implicit operator FloatRange(Vector4 v) {
			return new FloatRange(v.x, v.y);
		}

		public static implicit operator FloatRange(Vector2Int v) {
			return new FloatRange(v.x, v.y);
		}

		public static implicit operator FloatRange(Vector3Int v) {
			return new FloatRange(v.x, v.y);
		}

		public static implicit operator FloatRange(Vector4Int v) {
			return new FloatRange(v.x, v.y);
		}

		//-----------------------------------------------------------------------------------------
		// Public Methods:
		//-----------------------------------------------------------------------------------------

		public float Lerp(float t) {
			return Mathf.Lerp(min, max, t);
		}

		public float InverseLerp(float value) {
			return Mathf.InverseLerp(min, max, value);
		}

		public float Clamp(float value) {
			return (min > max) ? Mathf.Clamp(value, max, min) : Mathf.Clamp(value, min, max);
		}

		public float Random() {
			return (min == max) ? min : UnityEngine.Random.Range(min, max);
		}

		public int RandomInt() {

			// int ranges are exclusive or max number hence we increment it one.
			return (min == max) ? (int)min : UnityEngine.Random.Range((int)min, (int)(max + 1));
		}

		public float Evaluate(AnimationCurve animationCurve, float t) {
			float curvedT = animationCurve.Evaluate(t);
			return Mathf.Lerp(min, max, curvedT);
		}
	}
}