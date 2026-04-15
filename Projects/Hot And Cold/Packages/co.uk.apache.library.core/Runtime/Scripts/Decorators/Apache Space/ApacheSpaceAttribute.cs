using System;
using UnityEngine;

namespace Apache.Core {

	[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
	public class ApacheSpaceAttribute : PropertyAttribute {

		//-----------------------------------------------------------------------------------------
		// Public Fields:
		//-----------------------------------------------------------------------------------------

		/// <summary>
		/// The spacing in pixels.
		/// </summary>
		public virtual float Height => 6;
	}
}