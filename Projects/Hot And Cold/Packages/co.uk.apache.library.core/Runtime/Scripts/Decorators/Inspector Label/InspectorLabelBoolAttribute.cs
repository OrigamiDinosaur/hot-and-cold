using System;

namespace Apache.Core {

	[AttributeUsage(AttributeTargets.Field)]
	public class InspectorLabelBoolAttribute : InspectorLabelAttribute {
		
		//-----------------------------------------------------------------------------------------
		// Constructors:
		//-----------------------------------------------------------------------------------------

		public InspectorLabelBoolAttribute() : base(InspectorLabelDisplayTypes.Bool) { }
	}
}