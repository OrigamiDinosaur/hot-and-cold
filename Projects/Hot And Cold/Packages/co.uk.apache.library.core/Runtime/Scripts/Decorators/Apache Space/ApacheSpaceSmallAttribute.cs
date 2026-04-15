using System;

namespace Apache.Core {

	[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
	public class ApacheSpaceSmallAttribute : ApacheSpaceAttribute {

		//-----------------------------------------------------------------------------------------
		// Public Fields:
		//-----------------------------------------------------------------------------------------

		/// <inheritdoc />
		public override float Height => 3;
	}
}