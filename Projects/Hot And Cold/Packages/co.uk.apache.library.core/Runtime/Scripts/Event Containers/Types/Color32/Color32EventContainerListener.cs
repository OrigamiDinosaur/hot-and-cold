using System;
using UnityEngine;
using UnityEngine.Events;

namespace Apache.Core {

	public class Color32EventContainerListener : EventContainerListenerT<Color32, Color32EventContainer,
		Color32EventContainerListener.UnityColor32Event> {

		//-----------------------------------------------------------------------------------------
		// Classes:
		//-----------------------------------------------------------------------------------------

		[Serializable]
		public class UnityColor32Event : UnityEvent<Color32> { }
	}
}