using System;
using UnityEngine;
using UnityEngine.Events;

namespace Apache.Core {

	public class RenderTextureEventContainerListener : EventContainerListenerT<RenderTexture, RenderTextureEventContainer, RenderTextureEventContainerListener.UnityRenderTextureEvent> {

		//-----------------------------------------------------------------------------------------
		// Classes:
		//-----------------------------------------------------------------------------------------

		[Serializable]
		public class UnityRenderTextureEvent : UnityEvent<RenderTexture> { }
	}
}