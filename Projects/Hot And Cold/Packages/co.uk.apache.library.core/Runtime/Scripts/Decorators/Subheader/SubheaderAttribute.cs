using System;
using UnityEngine;

namespace Apache.Core {

	[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
	public class SubheaderAttribute : PropertyAttribute {

		//-----------------------------------------------------------------------------------------
		// Public Properties:
		//-----------------------------------------------------------------------------------------

		/// <summary>
		/// The subheader text.
		/// </summary>
		public string Subheader { get; }

		//-----------------------------------------------------------------------------------------
		// Public Methods:
		//-----------------------------------------------------------------------------------------

		/// <summary>
		/// Add a Subheader above fields in the Inspector.
		/// </summary>
		/// <param name="subheader">The Subheader text.</param>
		public SubheaderAttribute(string subheader) {
			Subheader = subheader;
		}
	}
}