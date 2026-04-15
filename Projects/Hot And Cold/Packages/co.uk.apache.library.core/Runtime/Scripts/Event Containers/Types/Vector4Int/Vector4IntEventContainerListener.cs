using System;
using UnityEngine.Events;

namespace Apache.Core {

	public class Vector4IntEventContainerListener : EventContainerListenerT<Vector4Int, Vector4IntEventContainer, Vector4IntEventContainerListener.UnityVector4IntEvent> {

		//-----------------------------------------------------------------------------------------
		// Classes:
		//-----------------------------------------------------------------------------------------

		[Serializable]
		public class UnityVector4IntEvent : UnityEvent<Vector4Int> { }
	}
}