using System;
using System.Text;

namespace Apache.Core.Extensions {
	public static class StringExtensions {

		//-----------------------------------------------------------------------------------------
		// Public Methods:
		//-----------------------------------------------------------------------------------------

		/// <summary>Converts the string from camel case (myStringValue) or pascal case (MyStringValue) to title case (My String Value).</summary>
		public static string CamelOrPascalToTitleCase(this string input) {

			// create string builder and always ensure the first letter is capitalised.
			StringBuilder sb = new StringBuilder(input.Length);
			sb.Append(char.ToUpper(input[0]));

			// starting from the second character, if we encounter a capital letter, simply add a space before it.
			for (int i = 1; i < input.Length; i++) {
				char c = input[i];
				if (char.IsUpper(c)) {
					sb.Append(' ');
				}
				sb.Append(c);
			}

			return sb.ToString();
		}

		/// <summary>
		/// Returns whether or not the specified string is contained with this string
		/// </summary>
		public static bool Contains(this string self, string toCheck, StringComparison comparisonType = StringComparison.InvariantCulture) {
			return self.IndexOf(toCheck, comparisonType) >= 0;
		}

		/// <summary>Replaces the first instance of <c>oldValue</c> with <c>newValue</c>.</summary>
		public static string ReplaceFirst(this string self, string oldValue, string newValue) {
			int position = self.IndexOf(oldValue, StringComparison.Ordinal);
			if (position < 0) {
				return self;
			}
			return self.Substring(0, position) + newValue + self.Substring(position + oldValue.Length);
		}

		/// <summary>Limits the string to the given number of characters and appends an ellipsis if clipping takes place.</summary>
		/// <remarks>If the limit is 20, and clipping takes place, the returned string will be 20 characters long, including the ellipsis.</remarks>
		/// <remarks>The <c>shouldUseThreeDots</c> argument is used to insist on an ellipsis comprised of three dots as opposed to the ellipsis unicode character (default).</remarks>
		public static string EllipsisClip(this string self, int charLimit, bool shouldUseThreeDots = false) {
			if (self.Length <= charLimit) return self;
			
			// create a string builder and append characters up to the limit (minus the space required for the ellipsis).
			StringBuilder outputBuilder = new StringBuilder();
			int ellipsisLength = (shouldUseThreeDots) ? 3 : 1;
			for (int i = 0; i < charLimit - ellipsisLength; i++) {
				if (self.Length > i) {
					outputBuilder.Append(self[i]);
				}
			}
			outputBuilder.Append((shouldUseThreeDots) ? "..." : "…");

			return outputBuilder.ToString();
		}

		/// <summary>Returns true if this string is null, empty, or contains only whitespace.</summary>
		/// <param name="self">The string to check.</param>
		/// <returns><c>true</c> if this string is null, empty, or contains only whitespace; otherwise, <c>false</c>.</returns>
		public static bool IsNullOrWhitespace(this string self) {

			if (string.IsNullOrEmpty(self)) return true;

			for (int i = 0; i < self.Length; i++) {
				if (char.IsWhiteSpace(self[i]) == false) {
					return false;
				}
			}

			return true;
		}
	}
}