using System;
using UnityEngine.Events;

namespace Apache.Core {

	public class
		BoolEventContainerListener : EventContainerListenerT<bool, BoolEventContainer, BoolEventContainerListener.UnityBoolEvent> {

		//-----------------------------------------------------------------------------------------
		// Classes:
		//-----------------------------------------------------------------------------------------

		[Serializable]
		public class UnityBoolEvent : UnityEvent<bool> { }
	}
}