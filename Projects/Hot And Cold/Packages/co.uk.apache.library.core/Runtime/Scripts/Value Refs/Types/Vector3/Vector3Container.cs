using UnityEngine;

namespace Apache.Core {

	[CreateAssetMenu(fileName = SCRIPTABLE_OBJECT_NAME, menuName = MENU_NAME_PREFIX + SCRIPTABLE_OBJECT_NAME, order = MENU_STARTING_ORDER + 16)]
	public class Vector3Container : ValueContainer<Vector3> {

		//-----------------------------------------------------------------------------------------
		// Constants:
		//-----------------------------------------------------------------------------------------

		private const string SCRIPTABLE_OBJECT_NAME = "Vector3 Container";
	}
}