using UnityEngine;

namespace Apache.Core {

	[CreateAssetMenu(fileName = SCRIPTABLE_OBJECT_NAME, menuName = EVENT_CONTAINER_MENU_NAME_PREFIX + SCRIPTABLE_OBJECT_NAME, order = MENU_STARTING_ORDER + 12)]
	public class IntEventContainer : EventContainer<int> {

		//-----------------------------------------------------------------------------------------
		// Constants:
		//-----------------------------------------------------------------------------------------

		private const string SCRIPTABLE_OBJECT_NAME = "Int Event Container";
	}
}