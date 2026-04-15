using System;
using UnityEngine;
using UnityEngine.Events;

namespace Apache.Core {

	public class Vector4EventContainerListener : EventContainerListenerT<Vector4, Vector4EventContainer, Vector4EventContainerListener.UnityVector4Event> {

		//-----------------------------------------------------------------------------------------
		// Classes:
		//-----------------------------------------------------------------------------------------

		[Serializable]
		public class UnityVector4Event : UnityEvent<Vector4> { }
	}
}