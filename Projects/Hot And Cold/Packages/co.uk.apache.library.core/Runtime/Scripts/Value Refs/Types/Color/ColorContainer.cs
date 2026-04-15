using UnityEngine;

namespace Apache.Core {

	[CreateAssetMenu(fileName = SCRIPTABLE_OBJECT_NAME, menuName = MENU_NAME_PREFIX + SCRIPTABLE_OBJECT_NAME, order = MENU_STARTING_ORDER + 31)]
	public class ColorContainer : ValueContainer<Color> {

		//-----------------------------------------------------------------------------------------
		// Constants:
		//-----------------------------------------------------------------------------------------

		private const string SCRIPTABLE_OBJECT_NAME = "Color Container";
	}
}