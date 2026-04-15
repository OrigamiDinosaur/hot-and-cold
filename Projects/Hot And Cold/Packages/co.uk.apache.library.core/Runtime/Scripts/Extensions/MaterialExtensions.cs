using System.Collections.Generic;
using UnityEngine;

namespace Apache.Core.Extensions {
	public static class MaterialExtensions {

		//-----------------------------------------------------------------------------------------
		// Public Methods:
		//-----------------------------------------------------------------------------------------

		/// <summary>Set this material's property by <c>propertyName</c> to the given value.</summary>
		public static void SetProperty(this Material self, string propertyName, float value) {
			self.SetFloat(propertyName, value);
		}

		/// <summary>Set this material's property by <c>propertyId</c> to the given value.</summary>
		public static void SetProperty(this Material self, int propertyId, float value) {
			self.SetFloat(propertyId, value);
		}

		/// <summary>Set this material's property by <c>propertyName</c> to the given value.</summary>
		public static void SetProperty(this Material self, string propertyName, ComputeBuffer value) {
			self.SetBuffer(propertyName, value);
		}

		/// <summary>Set this material's property by <c>propertyId</c> to the given value.</summary>
		public static void SetProperty(this Material self, int propertyId, ComputeBuffer value) {
			self.SetBuffer(propertyId, value);
		}

		/// <summary>Set this material's property by <c>propertyName</c> to the given value.</summary>
		public static void SetProperty(this Material self, string propertyName, Color value) {
			self.SetColor(propertyName, value);
		}

		/// <summary>Set this material's property by <c>propertyId</c> to the given value.</summary>
		public static void SetProperty(this Material self, int propertyId, Color value) {
			self.SetColor(propertyId, value);
		}

		/// <summary>Set this material's property by <c>propertyName</c> to the given value.</summary>
		public static void SetProperty(this Material self, string propertyName, Color[] value) {
			self.SetColorArray(propertyName, value);
		}

		/// <summary>Set this material's property by <c>propertyId</c> to the given value.</summary>
		public static void SetProperty(this Material self, int propertyId, Color[] value) {
			self.SetColorArray(propertyId, value);
		}

		/// <summary>Set this material's property by <c>propertyName</c> to the given value.</summary>
		public static void SetProperty(this Material self, string propertyName, List<Color> value) {
			self.SetColorArray(propertyName, value);
		}

		/// <summary>Set this material's property by <c>propertyId</c> to the given value.</summary>
		public static void SetProperty(this Material self, int propertyId, List<Color> value) {
			self.SetColorArray(propertyId, value);
		}

		/// <summary>Set this material's property by <c>propertyName</c> to the given value.</summary>
		public static void SetProperty(this Material self, string propertyName, float[] value) {
			self.SetFloatArray(propertyName, value);
		}

		/// <summary>Set this material's property by <c>propertyId</c> to the given value.</summary>
		public static void SetProperty(this Material self, int propertyId, float[] value) {
			self.SetFloatArray(propertyId, value);
		}

		/// <summary>Set this material's property by <c>propertyName</c> to the given value.</summary>
		public static void SetProperty(this Material self, string propertyName, List<float> value) {
			self.SetFloatArray(propertyName, value);
		}

		/// <summary>Set this material's property by <c>propertyId</c> to the given value.</summary>
		public static void SetProperty(this Material self, int propertyId, List<float> value) {
			self.SetFloatArray(propertyId, value);
		}

		/// <summary>Set this material's property by <c>propertyName</c> to the given value.</summary>
		public static void SetProperty(this Material self, string propertyName, int value) {
			self.SetInt(propertyName, value);
		}

		/// <summary>Set this material's property by <c>propertyId</c> to the given value.</summary>
		public static void SetProperty(this Material self, int propertyId, int value) {
			self.SetInt(propertyId, value);
		}

		/// <summary>Set this material's property by <c>propertyName</c> to the given value.</summary>
		public static void SetProperty(this Material self, string propertyName, Matrix4x4 value) {
			self.SetMatrix(propertyName, value);
		}

		/// <summary>Set this material's property by <c>propertyId</c> to the given value.</summary>
		public static void SetProperty(this Material self, int propertyId, Matrix4x4 value) {
			self.SetMatrix(propertyId, value);
		}

		/// <summary>Set this material's property by <c>propertyName</c> to the given value.</summary>
		public static void SetProperty(this Material self, string propertyName, Matrix4x4[] value) {
			self.SetMatrixArray(propertyName, value);
		}

		/// <summary>Set this material's property by <c>propertyId</c> to the given value.</summary>
		public static void SetProperty(this Material self, int propertyId, Matrix4x4[] value) {
			self.SetMatrixArray(propertyId, value);
		}

		/// <summary>Set this material's property by <c>propertyName</c> to the given value.</summary>
		public static void SetProperty(this Material self, string propertyName, List<Matrix4x4> value) {
			self.SetMatrixArray(propertyName, value);
		}

		/// <summary>Set this material's property by <c>propertyId</c> to the given value.</summary>
		public static void SetProperty(this Material self, int propertyId, List<Matrix4x4> value) {
			self.SetMatrixArray(propertyId, value);
		}

		/// <summary>Set this material's property by <c>propertyName</c> to the given value.</summary>
		public static void SetProperty(this Material self, string propertyName, Texture value) {
			self.SetTexture(propertyName, value);
		}

		/// <summary>Set this material's property by <c>propertyId</c> to the given value.</summary>
		public static void SetProperty(this Material self, int propertyId, Texture value) {
			self.SetTexture(propertyId, value);
		}

		/// <summary>Set this material's property by <c>propertyName</c> to the given value.</summary>
		public static void SetProperty(this Material self, string propertyName, Vector4 value) {
			self.SetVector(propertyName, value);
		}

		/// <summary>Set this material's property by <c>propertyId</c> to the given value.</summary>
		public static void SetProperty(this Material self, int propertyId, Vector4 value) {
			self.SetVector(propertyId, value);
		}

		/// <summary>Set this material's property by <c>propertyName</c> to the given value.</summary>
		public static void SetProperty(this Material self, string propertyName, Vector4[] value) {
			self.SetVectorArray(propertyName, value);
		}

		/// <summary>Set this material's property by <c>propertyId</c> to the given value.</summary>
		public static void SetProperty(this Material self, int propertyId, Vector4[] value) {
			self.SetVectorArray(propertyId, value);
		}

		/// <summary>Set this material's property by <c>propertyName</c> to the given value.</summary>
		public static void SetProperty(this Material self, string propertyName, List<Vector4> value) {
			self.SetVectorArray(propertyName, value);
		}

		/// <summary>Set this material's property by <c>propertyId</c> to the given value.</summary>
		public static void SetProperty(this Material self, int propertyId, List<Vector4> value) {
			self.SetVectorArray(propertyId, value);
		}

	}
}