using System;
using UnityEngine.Events;

namespace Apache.Core {

	public class FloatRangeEventContainerListener : EventContainerListenerT<FloatRange, FloatRangeEventContainer, FloatRangeEventContainerListener.UnityFloatRangeEvent> {

		//-----------------------------------------------------------------------------------------
		// Classes:
		//-----------------------------------------------------------------------------------------

		[Serializable]
		public class UnityFloatRangeEvent : UnityEvent<FloatRange> { }
	}
}