using System;
using UnityEngine;
using UnityEngine.Events;

namespace Apache.Core {

	public class Texture2DEventContainerListener : EventContainerListenerT<Texture2D, Texture2DEventContainer, Texture2DEventContainerListener.UnityTexture2DEvent> {

		//-----------------------------------------------------------------------------------------
		// Classes:
		//-----------------------------------------------------------------------------------------

		[Serializable]
		public class UnityTexture2DEvent : UnityEvent<Texture2D> { }
	}
}