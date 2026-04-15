using System;
using UnityEngine;

namespace Apache.Core {

	[Serializable]
	public abstract class ValueContainer<T> : ApacheScriptableObject, ISerializationCallbackReceiver {

		//-----------------------------------------------------------------------------------------
		// Constants:
		//-----------------------------------------------------------------------------------------

		protected new const string MENU_NAME_PREFIX = ApacheScriptableObject.MENU_NAME_PREFIX + "Value Containers/";

		//-----------------------------------------------------------------------------------------
		// Delegates:
		//-----------------------------------------------------------------------------------------

		public delegate void ChangedEventHandler(T value);

		//-----------------------------------------------------------------------------------------
		// Events:
		//-----------------------------------------------------------------------------------------

		public event ChangedEventHandler Changed;

		//-----------------------------------------------------------------------------------------
		// Inspector Variables:
		//-----------------------------------------------------------------------------------------

		[SerializeField] protected T initialValue;

		//-----------------------------------------------------------------------------------------
		// Inspector Variables:
		//-----------------------------------------------------------------------------------------

		[NonSerialized] protected T runtimeValue;

		//-----------------------------------------------------------------------------------------
		// Public Properties:
		//-----------------------------------------------------------------------------------------

		/// <remarks>
		/// The value is the most important thing and will be accessed countless times when using a <c>ValueContainer</c>.
		/// For this reason, it makes sense to abbreviate the property to just <c>Val</c>, for brevity.
		/// </remarks>
		public T Val {
			get => runtimeValue;
			set {

				// ignore a set where the value is not changing.
				if (Equals(Val, value)) return;

				// update value and invoke changed event.
				runtimeValue = value;

				// invoke changed event.
				Changed?.Invoke(Val);
			}
		}

		//-----------------------------------------------------------------------------------------
		// Operator Overloads:
		//-----------------------------------------------------------------------------------------

		public static implicit operator T(ValueContainer<T> container) {
			return container.Val;
		}

		//-----------------------------------------------------------------------------------------
		// Public Methods:
		//-----------------------------------------------------------------------------------------

#if UNITY_EDITOR
		[ApacheButton]
		public void InvokeChangedEvent() {
			Changed?.Invoke(runtimeValue);
		}
#endif

		//-----------------------------------------------------------------------------------------
		// Serialization Methods
		//-----------------------------------------------------------------------------------------

		public void OnBeforeSerialize() { }

		public void OnAfterDeserialize() {

			// sets our runtime value to the newly declared value.
			runtimeValue = initialValue;
		}
	}
}