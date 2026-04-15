using System;
using UnityEngine;
using UnityEngine.Events;

namespace Apache.Core {

	public class Vector3IntEventContainerListener : EventContainerListenerT<Vector3Int, Vector3IntEventContainer, Vector3IntEventContainerListener.UnityVector3IntEvent> {

		//-----------------------------------------------------------------------------------------
		// Classes:
		//-----------------------------------------------------------------------------------------

		[Serializable]
		public class UnityVector3IntEvent : UnityEvent<Vector3Int> { }
	}
}