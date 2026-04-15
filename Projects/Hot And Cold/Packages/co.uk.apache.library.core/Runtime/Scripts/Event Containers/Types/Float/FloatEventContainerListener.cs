using System;
using UnityEngine.Events;

namespace Apache.Core {

	public class FloatEventContainerListener : EventContainerListenerT<float, FloatEventContainer, FloatEventContainerListener.UnityFloatEvent> {

		//-----------------------------------------------------------------------------------------
		// Classes:
		//-----------------------------------------------------------------------------------------

		[Serializable]
		public class UnityFloatEvent : UnityEvent<float> { }
	}
}