using System;
using UnityEngine;
using UnityEngine.Events;

namespace Apache.Core {

	public class ColorEventContainerListener : EventContainerListenerT<Color, ColorEventContainer, ColorEventContainerListener.UnityColorEvent> {

		//-----------------------------------------------------------------------------------------
		// Classes:
		//-----------------------------------------------------------------------------------------

		[Serializable]
		public class UnityColorEvent : UnityEvent<Color> { }
	}
}