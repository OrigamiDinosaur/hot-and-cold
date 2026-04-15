
namespace Apache.Core {

	[System.AttributeUsage(System.AttributeTargets.Method)]
	public class ApacheButtonAttribute : System.Attribute {

		//-----------------------------------------------------------------------------------------
		// Public Properties:
		//-----------------------------------------------------------------------------------------

		public string Label;
		public bool ShouldOnlyShowInPlayMode;

		//-----------------------------------------------------------------------------------------
		// Constructors:
		//-----------------------------------------------------------------------------------------

		public ApacheButtonAttribute(string label, bool shouldOnlyShowInPlayMode = false) {
			Label = label;
			ShouldOnlyShowInPlayMode = shouldOnlyShowInPlayMode;
		}

		public ApacheButtonAttribute(bool shouldOnlyShowInPlayMode = false) {
			ShouldOnlyShowInPlayMode = shouldOnlyShowInPlayMode;
		}
	}
}