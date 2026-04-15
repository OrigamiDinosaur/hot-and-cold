using System;
using UnityEngine;
using UnityEngine.Events;

namespace Apache.Core {

	public class Vector3EventContainerListener : EventContainerListenerT<Vector3, Vector3EventContainer, Vector3EventContainerListener.UnityVector3Event> {

		//-----------------------------------------------------------------------------------------
		// Classes:
		//-----------------------------------------------------------------------------------------

		[Serializable]
		public class UnityVector3Event : UnityEvent<Vector3> { }
	}
}