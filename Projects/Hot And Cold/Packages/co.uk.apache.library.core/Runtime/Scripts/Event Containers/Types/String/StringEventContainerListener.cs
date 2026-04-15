using System;
using UnityEngine.Events;

namespace Apache.Core {

	public class StringEventContainerListener : EventContainerListenerT<string, StringEventContainer, StringEventContainerListener.UnityStringEvent> {

		//-----------------------------------------------------------------------------------------
		// Classes:
		//-----------------------------------------------------------------------------------------

		[Serializable]
		public class UnityStringEvent : UnityEvent<string> { }
	}
}