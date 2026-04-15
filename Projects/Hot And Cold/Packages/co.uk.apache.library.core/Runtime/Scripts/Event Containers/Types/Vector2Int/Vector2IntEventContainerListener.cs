using System;
using UnityEngine;
using UnityEngine.Events;

namespace Apache.Core {

	public class Vector2IntEventContainerListener : EventContainerListenerT<Vector2Int, Vector2IntEventContainer, Vector2IntEventContainerListener.UnityVector2IntEvent> {

		//-----------------------------------------------------------------------------------------
		// Classes:
		//-----------------------------------------------------------------------------------------

		[Serializable]
		public class UnityVector2IntEvent : UnityEvent<Vector2Int> { }
	}
}