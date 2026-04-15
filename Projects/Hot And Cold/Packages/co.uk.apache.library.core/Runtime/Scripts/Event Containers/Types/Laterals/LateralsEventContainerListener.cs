using System;
using UnityEngine.Events;

namespace Apache.Core {

	public class LateralsEventContainerListener : EventContainerListenerT<Laterals, LateralsEventContainer, LateralsEventContainerListener.UnityLateralsEvent> {

		//-----------------------------------------------------------------------------------------
		// Classes:
		//-----------------------------------------------------------------------------------------

		[Serializable]
		public class UnityLateralsEvent : UnityEvent<Laterals> { }
	}
}