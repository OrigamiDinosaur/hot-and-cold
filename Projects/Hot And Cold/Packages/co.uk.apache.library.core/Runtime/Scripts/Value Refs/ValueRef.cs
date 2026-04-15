using UnityEngine;

namespace Apache.Core {

	public class ValueRef<TValue, TContainer> where TContainer : ValueContainer<TValue> {

		//-----------------------------------------------------------------------------------------
		// Delegates:
		//-----------------------------------------------------------------------------------------

		public delegate void ChangedEventHandler(TValue value);

		//-----------------------------------------------------------------------------------------
		// Events:
		//-----------------------------------------------------------------------------------------

		public event ChangedEventHandler Changed {
			add {
				_Changed += value;

				// ignore this subscription if we don't have a container or the container is unchanged.
				if (container == null || prevSubscribedToChangedContainer == container) return;

				// if we have previous container (which is different to the existing), unwire changed delegate.
				if (prevSubscribedToChangedContainer != null) {
					prevSubscribedToChangedContainer.Changed -= Container_Changed;
				}

				// subscribe to changed on existing container and keep track of it.
				container.Changed += Container_Changed;
				prevSubscribedToChangedContainer = container;
			}

			remove {
				// ReSharper disable once ArrangeAccessorOwnerBody
				_Changed -= value;
			}
		}

		// ReSharper disable once InconsistentNaming

		private event ChangedEventHandler _Changed;

		//-----------------------------------------------------------------------------------------
		// Inspector Variables:
		//-----------------------------------------------------------------------------------------

		[SerializeField] protected TValue value;
		[SerializeField] protected bool shouldUseContainer;
		[SerializeField] protected TContainer container;

		//-----------------------------------------------------------------------------------------
		// Private Fields:
		//-----------------------------------------------------------------------------------------

		private TContainer prevSubscribedToChangedContainer;

		//-----------------------------------------------------------------------------------------
		// Public Properties:
		//-----------------------------------------------------------------------------------------

		/// <remarks>
		/// The value is the most important thing and will be accessed countless times when using a <c>ValueRef</c>.
		/// For this reason, it makes sense to abbreviate the property to just <c>Val</c>, for brevity.
		/// </remarks>
		public TValue Val {
			get => (shouldUseContainer) ? container.Val : value;
			set {

				// ignore a set where the value is not changing.
				if (Equals(Val, value)) return;

				// update reference or value.
				if (shouldUseContainer) {
					container.Val = value;
				}
				else {
					this.value = value;
				}

				// invoke changed event.
				_Changed?.Invoke(Val);
			}
		}

		public TContainer Container {
			get => container;
			set => container = value;
		}

		//-----------------------------------------------------------------------------------------
		// Event Handlers:
		//-----------------------------------------------------------------------------------------

		private void Container_Changed(TValue newValue) {
			_Changed?.Invoke(Val);
		}
	}
}