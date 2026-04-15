using System;
using UnityEngine;
using UnityEngine.Events;

namespace Apache.Core {

	public class QuaternionEventContainerListener : EventContainerListenerT<Quaternion, QuaternionEventContainer, QuaternionEventContainerListener.UnityQuaternionEvent> {

		//-----------------------------------------------------------------------------------------
		// Classes:
		//-----------------------------------------------------------------------------------------

		[Serializable]
		public class UnityQuaternionEvent : UnityEvent<Quaternion> { }
	}
}