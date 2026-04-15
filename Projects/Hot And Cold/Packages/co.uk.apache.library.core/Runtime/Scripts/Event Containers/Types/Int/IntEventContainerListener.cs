using System;
using UnityEngine.Events;

namespace Apache.Core {

	public class IntEventContainerListener : EventContainerListenerT<int, IntEventContainer, IntEventContainerListener.UnityIntEvent> {

		//-----------------------------------------------------------------------------------------
		// Classes:
		//-----------------------------------------------------------------------------------------

		[Serializable]
		public class UnityIntEvent : UnityEvent<int> { }
	}
}