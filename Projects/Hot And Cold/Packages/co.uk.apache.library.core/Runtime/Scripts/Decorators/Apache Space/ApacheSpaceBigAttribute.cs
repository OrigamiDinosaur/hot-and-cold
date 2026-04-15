using System;

namespace Apache.Core {

	[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
	public class ApacheSpaceBigAttribute : ApacheSpaceAttribute {

		//-----------------------------------------------------------------------------------------
		// Public Fields:
		//-----------------------------------------------------------------------------------------

		/// <inheritdoc />
		public override float Height => 10;
	}
}