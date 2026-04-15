using UnityEngine;

namespace Apache.Core.Extensions {
	public static class FloatExtensions {

		//-----------------------------------------------------------------------------------------
		// Public Methods:
		//-----------------------------------------------------------------------------------------

		/// <summary>Round the float using <c>Mathf.Round</c>.</summary>
		public static float Round(this float self) {
			return Mathf.Round(self);
		}

		/// <summary>Round the float using <c>Mathf.Round</c> to the nearest given interval.</summary>
		public static float Round(this float self, float toNearest) {
			if (toNearest == 0) return self;
			return Mathf.Round(self * toNearest) / toNearest;
		}

		/// <summary>Converts angles to within the range of 0 to 360.</summary>
		public static float ToAngle360(this float self) {
			if (self >= 0) return self % 360;
			return (360 + self) % 360;
		}
	}
}