using UnityEngine;
using Color = UnityEngine.Color;

namespace Apache.Core.Extensions {
	public static class GraphicsExtensions {

		//-----------------------------------------------------------------------------------------
		// Public Methods:
		//-----------------------------------------------------------------------------------------

		/// <summary>Returns a <c>Vector2Int</c> denoting the size (width and height) of the texture.</summary>
		public static Vector2Int Size(this Texture self) {
			return new Vector2Int(self.width, self.height);
		}

		/// <summary>Determines if a texture is readable by attempting to read a single pixel.</summary>
		public static bool IsReadable(this Texture2D self) {
			try {
				self.GetPixel(0, 0);
				return true;
			}
			catch {
				return false;
			}
		}

		/// <summary>
		/// Converts a <c>RenderTexture</c> to a <c>Texture2D</c>.
		/// </summary>
		/// <param name="renderTexture">The <c>RenderTexture</c> to convert.</param>
		/// <param name="shouldUseMipmaps">Whether or not to use mipmaps.</param>
		/// <param name="isLinear">Whether the <c>RenderTexture</c> is in the linear colour space.</param>
		/// <returns>The converted to <c>Texture2D</c>.</returns>
		public static Texture2D ToTexture2D(this RenderTexture renderTexture, bool shouldUseMipmaps = false, bool isLinear = true) {
			
			// instantiate the texture at the size of the render texture.
			Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, shouldUseMipmaps, isLinear);

			// assign the render texture active, grabbing the current active to reinstate later.
			RenderTexture prevRenderTexture = RenderTexture.active;
			RenderTexture.active = renderTexture;

			// read the active render texture pixels into the texture and apply it.
			texture.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0, shouldUseMipmaps);
			texture.Apply();

			// reinstate the previous active render texture.
			RenderTexture.active = prevRenderTexture;

			return texture;
		}
		
		/// <summary>Fills all pixels of the <c>Texture2D</c> with the given colour.</summary>
		public static void Fill(this Texture2D self, Color colour) {
			for (int x = 0; x < self.width; ++x) {
				for (int y = 0; y < self.height; ++y) {
					self.SetPixel(x, y, colour);
				}
			}
		}
	}
}