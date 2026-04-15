using System;
using UnityEngine;
using UnityEngine.Events;

namespace Apache.Core {

	public class Vector2EventContainerListener : EventContainerListenerT<Vector2, Vector2EventContainer, Vector2EventContainerListener.UnityVector2Event> {

		//-----------------------------------------------------------------------------------------
		// Classes:
		//-----------------------------------------------------------------------------------------

		[Serializable]
		public class UnityVector2Event : UnityEvent<Vector2> { }
	}
}