using UnityEngine;

namespace Apache.Core {

	[CreateAssetMenu(fileName = SCRIPTABLE_OBJECT_NAME, menuName = MENU_NAME_PREFIX + SCRIPTABLE_OBJECT_NAME, order = MENU_STARTING_ORDER + 17)]
	public class Vector4Container : ValueContainer<Vector4> {

		//-----------------------------------------------------------------------------------------
		// Constants:
		//-----------------------------------------------------------------------------------------

		private const string SCRIPTABLE_OBJECT_NAME = "Vector4 Container";
	}
}