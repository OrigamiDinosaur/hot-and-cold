namespace Apache.Core.Extensions.Enum {

	using System;

	public static class EnumExtensions {

		//-----------------------------------------------------------------------------------------
		// Public Methods:
		//-----------------------------------------------------------------------------------------

		/// <summary>
		/// Returns the next or first (if the current is the last) enumeration value.
		/// </summary>
		/// <typeparam name="TEnum">The generic enumeration type.</typeparam>
		/// <remarks>The generic type name is <c>TEnum</c> to emphasise that it should be (but could conceivably not be) an enum.</remarks>
		/// <remarks>The generic type constaints <c>IConvertible</c>, <c>IComparable</c>, <c>IFormattable</c> are all the interfaces which Enum implements, making it
		/// less likely that this method is available to extend other types.</remarks>
		public static TEnum NextOrFirst<TEnum>(this TEnum self) where TEnum : struct, IConvertible, IComparable, IFormattable {

			CheckIsEnum<TEnum>();

			// convert the possible enum values into an array of values and chose the next or first.
			TEnum[] array = (TEnum[])Enum.GetValues(self.GetType());
			int i = Array.IndexOf(array, self) + 1;
			return (i == array.Length) ? array[0] : array[i];
		}

		/// <summary>
		/// Returns the previous or last (if the current is the first) enumeration value.
		/// </summary>
		/// <typeparam name="TEnum">The generic enumeration type.</typeparam>
		/// <remarks>The generic type name is <c>TEnum</c> to emphasise that it should be (but could conceivably not be) an enum.</remarks>
		/// <remarks>The generic type constaints <c>IConvertible</c>, <c>IComparable</c>, <c>IFormattable</c> are all the interfaces which Enum implements, making it
		/// less likely that this method is available to extend other types.</remarks>
		public static TEnum PrevOrLast<TEnum>(this TEnum self) where TEnum : struct, IConvertible, IComparable, IFormattable {

			CheckIsEnum<TEnum>();

			// convert the possible enum values into an array of values and chose the previous or last.
			TEnum[] array = (TEnum[])Enum.GetValues(self.GetType());
			int i = Array.IndexOf(array, self) - 1;
			return (i == -1) ? array[array.Length - 1] : array[i];
		}

		//-----------------------------------------------------------------------------------------
		// Private Methods:
		//-----------------------------------------------------------------------------------------

		/// <summary>
		/// Because we can't constrain on where TEnum : Enum, we have to check for accidental misuse.
		/// </summary>
		/// <typeparam name="TEnum">The type to check whether it is an enum.</typeparam>
		private static void CheckIsEnum<TEnum>() {
			if (!typeof(TEnum).IsEnum) {
				throw new NotSupportedException($"Argument { typeof(TEnum).FullName } is not an Enum.");
			}
		}
	}
}