using UnityEngine;

namespace Apache.Core.Extensions {
	public static class ColorExtensions {

		//-----------------------------------------------------------------------------------------
		// Public Methods:
		//-----------------------------------------------------------------------------------------

		/// <summary>Moves towards a given colour with a maximum movement delta.</summary>
		public static Color MoveTowards(this Color self, Color target, float maxDelta) {
			self.r = Mathf.MoveTowards(self.r, target.r, maxDelta);
			self.g = Mathf.MoveTowards(self.g, target.g, maxDelta);
			self.b = Mathf.MoveTowards(self.b, target.b, maxDelta);
			self.a = Mathf.MoveTowards(self.a, target.a, maxDelta);
			return self;
		}
		
		/// <summary>Raises the colour values to the given exponent.</summary>
		public static Color Pow(this Color self, float exponent) {
			self.r = Mathf.Pow(self.r, exponent);
			self.g = Mathf.Pow(self.g, exponent);
			self.b = Mathf.Pow(self.b, exponent);
			self.a = Mathf.Pow(self.a, exponent);
			return self;
		}

		/// <summary>Normalises the RGB values of the colour while leaving the alpha value untouched.</summary>
		public static Color NormaliseRgb(this Color self) {
			Vector3 c = new Vector3(self.r, self.g, self.b).normalized;
			self.r = c.x;
			self.g = c.y;
			self.b = c.z;
			return self;
		}
		
		/// <summary>Returns the given colour with the given alpha.</summary>
		public static Color WithAlpha(this Color self, float alpha) {
			self.a = alpha;
			return self;
		}
	}
}