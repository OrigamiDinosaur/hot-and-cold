using System;
using UnityEngine;
using UnityEngine.Events;

namespace Apache.Core {

	public class TextureEventContainerListener : EventContainerListenerT<Texture, TextureEventContainer, TextureEventContainerListener.UnityTextureEvent> {

		//-----------------------------------------------------------------------------------------
		// Classes:
		//-----------------------------------------------------------------------------------------

		[Serializable]
		public class UnityTextureEvent : UnityEvent<Texture> { }
	}
}