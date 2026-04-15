using System.Linq;
using System.Text;
using UnityEngine;

public static class ColourUtils {
	
	//-----------------------------------------------------------------------------------------
	// Public Methods:
	//-----------------------------------------------------------------------------------------
	
	/// <summary>Lerp between colours <c>a</c> and <c>b</c> by <c>t</c> without clamping.</summary>
	public static Color UnclampedLerp(Color a, Color b, float t) {
		return new Color(a.r + (b.r - a.r) * t, a.g + (b.g - a.g) * t, a.b + (b.b - a.b) * t, a.a + (b.a - a.a) * t);
	}

	/// <summary>Attempts to parse the given hex code string into a colour.</summary>
	/// <remarks>Examples of supported formats include #FFF, #FFF0, #FFFFFF, #FFFFFF00, and 0xFFFFFF.</remarks>
	public static bool TryParseHex(string hexString, out Color colour) {

		hexString = hexString.Replace("0x", string.Empty); // in case the string is formatted 0xFFFFFF.
		hexString = hexString.Replace("#",  string.Empty); // in case the string is formatted #FFFFFF.

		// ReSharper disable InlineOutVariableDeclaration
		byte r, g, b;
		// ReSharper restore InlineOutVariableDeclaration
		byte a = 255; // assume fully visible unless specified in hex.

		// ReSharper disable once RedundantTypeSpecificationInDefaultExpression
		colour = default(Color);

		// fix compressed hex colours, like #FFF or #0000, by assuming three or four length hexes should be expanded out by doubling each character.
		if (hexString.Length == 3 || hexString.Length == 4) {
			StringBuilder hexStringBuilder = new StringBuilder();
			string[] hexStringCharacters = hexString.ToCharArray().Select(c => c.ToString()).ToArray();
			foreach (string hexStringCharacter in hexStringCharacters) {
				hexStringBuilder.Append(hexStringCharacter);
				hexStringBuilder.Append(hexStringCharacter);
			}
			hexString = hexStringBuilder.ToString();
		}

		if (!(hexString.Length == 6 | hexString.Length == 8)) return false;

		if (!byte.TryParse(hexString.Substring(0, 2), System.Globalization.NumberStyles.HexNumber, null, out r)) return false;
		if (!byte.TryParse(hexString.Substring(2, 2), System.Globalization.NumberStyles.HexNumber, null, out g)) return false;
		if (!byte.TryParse(hexString.Substring(4, 2), System.Globalization.NumberStyles.HexNumber, null, out b)) return false;

		// only use alpha if the string has enough characters.
		if (hexString.Length == 8) {
			if (!byte.TryParse(hexString.Substring(6, 2), System.Globalization.NumberStyles.HexNumber, null, out a)) return false;
		}

		colour = new Color32(r, g, b, a);
		return true;
	}
}