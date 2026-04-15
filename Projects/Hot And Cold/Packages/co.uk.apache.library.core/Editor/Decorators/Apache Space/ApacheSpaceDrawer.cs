using UnityEditor;

namespace Apache.Core.Editor {

	[CustomPropertyDrawer(typeof(ApacheSpaceAttribute), true)]
	public class ApacheSpaceDrawer : DecoratorDrawer {

		//-----------------------------------------------------------------------------------------
		// Public Methods:
		//-----------------------------------------------------------------------------------------

		public override float GetHeight() {
			ApacheSpaceAttribute apacheSpace = (attribute as ApacheSpaceAttribute);
			return apacheSpace?.Height ?? 0;
		}
	}
}