using UnityEngine;
using UnityEngine.Events;

namespace Apache.Core {

	public class EventContainerListener : ApacheComponent {

		//-----------------------------------------------------------------------------------------
		// Inspector Variables:
		//-----------------------------------------------------------------------------------------

		[SerializeField] protected EventContainer container;
		[SerializeField] protected UnityEvent @event;

		//-----------------------------------------------------------------------------------------
		// Unity Lifecycle:
		//-----------------------------------------------------------------------------------------

		protected void OnEnable() {
			if (container == null) return;
			container.Event += Container_Event;
		}

#if UNITY_EDITOR
		protected void OnValidate() {

			// if we don't have an event object yet, or we already have listeners wired up, back out.
			if (container == null || @event.GetPersistentEventCount() > 0) return;

			// grab the first Apache Component.
			ApacheComponent apacheComponent = GetComponent<ApacheComponent>();
			if (apacheComponent == null) return;

			// wire up a null persistent listener, just to get things started.
			UnityEditor.Events.UnityEventTools.AddPersistentListener(@event, apacheComponent.NullEventListener);
		}
#endif

		protected void OnDisable() {
			if (container == null) return;
			container.Event -= Container_Event;
		}

		//-----------------------------------------------------------------------------------------
		// Event Handlers:
		//-----------------------------------------------------------------------------------------

		private void Container_Event() {
			@event.Invoke();
		}
	}

	public class EventContainerListenerT<T, TEventContainer, TUnityEvent> : ApacheComponent
		where TEventContainer : EventContainer<T>
		where TUnityEvent : UnityEvent<T> {

		//-----------------------------------------------------------------------------------------
		// Inspector Variables:
		//-----------------------------------------------------------------------------------------

		[SerializeField] protected TEventContainer container;
		[SerializeField] protected TUnityEvent @event;

		//-----------------------------------------------------------------------------------------
		// Unity Lifecycle:
		//-----------------------------------------------------------------------------------------

		protected void OnEnable() {
			if (container == null) return;
			container.Event += Container_Event;
		}

		// N.B. sadly, we can't wire up a generic event listener, so we have to forego automatic wire up behaviour in OnValidate.

		protected void OnDisable() {
			if (container == null) return;
			container.Event -= Container_Event;
		}

		//-----------------------------------------------------------------------------------------
		// Event Handlers:
		//-----------------------------------------------------------------------------------------

		private void Container_Event(T value) {
			@event.Invoke(value);
		}
	}
}