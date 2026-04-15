using System;
using UnityEngine.Events;

namespace Apache.Core {

	public class DoubleEventContainerListener : EventContainerListenerT<double, DoubleEventContainer, UnityDoubleEvent> { }

	//-----------------------------------------------------------------------------------------
	// Classes:
	//-----------------------------------------------------------------------------------------

	[Serializable]
	public class UnityDoubleEvent : UnityEvent<double> { }
}