using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Apache.Core {
	public static class Pool<T> where T : ApacheComponent, IPoolable {

		//-----------------------------------------------------------------------------------------
		// Constants:
		//-----------------------------------------------------------------------------------------

		private const bool DEFAULT_IS_DYNAMIC = true;

		//-----------------------------------------------------------------------------------------
		// Private Fields:
		//-----------------------------------------------------------------------------------------

		// ReSharper disable StaticMemberInGenericType

		private static bool hasInited;

		private static T prototype;
		private static bool isDynamic = DEFAULT_IS_DYNAMIC;

		private static HashSet<PooledItem> pooledItems;

		private static PoolHandler handler;

		// ReSharper restore StaticMemberInGenericType

		//-----------------------------------------------------------------------------------------
		// Event Handlers:
		//-----------------------------------------------------------------------------------------

		private static void Handler_Destroyed() {

			// first things first, unsubscribe from the event now that we've received it.
			handler.Destroyed -= Handler_Destroyed;

			// when the handler is destroyed, make sure to reset and clear out all fields so we're safe to reinit.
			prototype = null;
			isDynamic = DEFAULT_IS_DYNAMIC;
			pooledItems.Clear();
			pooledItems = null;
			handler = null;

			// finally flag we're not ready to be reinitialised.
			hasInited = false;
		}

		//-----------------------------------------------------------------------------------------
		// Public Methods:
		//-----------------------------------------------------------------------------------------

		// ReSharper disable ParameterHidesMember

		public static void Init(T prototype, int? prewarmWithNum = null, bool isDynamic = DEFAULT_IS_DYNAMIC) {

			// ensure we don't try to double init.
			Assert.IsFalse(hasInited);

			// grab prototype and ensure its disabled.
			Pool<T>.prototype = prototype;
			Pool<T>.prototype.gameObject.SetActive(false);

			Pool<T>.isDynamic = isDynamic;

			// initialise the pooled items collection.
			pooledItems = new HashSet<PooledItem>();

			// initialise the handler and subscribe to its destroyed event.
			handler = PoolHandler.Init<T>();
			handler.Destroyed += Handler_Destroyed;

			// if we're prewarming, do so now.
			if (prewarmWithNum != null) {
				Prewarm((int)prewarmWithNum);
			}

			hasInited = true;
		}

		// ReSharper restore ParameterHidesMember

		/// <summary>
		/// Retrieve an object from the pool with default values.
		/// </summary>
		public static T Retrieve() {
			return (RetrievePoolable());
		}

		/// <summary>
		/// Retrieve an object from the pool with a defined parent.
		/// </summary>
		/// <param name="parent">The object that the newly returned retrievable will be childed to.</param>
		/// <param name="shouldWorldPositionStay">If true the object will maintain its world position when childed to its parent. Otherwise it will be translated to be relative to parent.</param>
		/// <returns></returns>
		public static T Retrieve(Transform parent, bool shouldWorldPositionStay = false) {

			// retrieve an item from the pool.
			T poolable = RetrievePoolable();

			// collect localPosition of poolable.
			Vector3 localPosition = poolable.transform.localPosition;

			// child our poolable to the parent.
			poolable.transform.parent = parent;

			// if we are instantiated in world space than return here, we have done our job.
			if (shouldWorldPositionStay) return poolable;

			// if we are not instantiating in world space than we apply our local position;
			poolable.transform.localPosition = localPosition;

			return poolable;
		}

		/// <summary>
		/// Retrieve an object from the pool with a defined positon.
		/// </summary>
		/// <param name="position">The position in world space the object will be placed at.</param>
		public static T Retrieve(Vector3 position) {

			// retrieve an item from the pool.
			T poolable = RetrievePoolable();

			// apply our position to the poolable.
			poolable.transform.position = position;

			//return our new object.
			return poolable;
		}

		/// <summary>
		/// Retrieve an object from the pool with a defined position and rotation.
		/// </summary>
		/// <param name="position">The position in world space the object will be placed at.</param>
		/// <param name="rotation">The rotation in world space the object will be oriented to.</param>
		public static T Retrieve(Vector3 position, Quaternion rotation) {

			// retrieve an item from the pool.
			T poolable = RetrievePoolable();

			// apply our position to the poolable and rotation.
			poolable.transform.position = position;
			poolable.transform.rotation = rotation;

			//return our new object.
			return poolable;
		}

		/// <summary>
		/// Retrieve an object from the pool with a defined position, rotation, and parent.
		/// </summary>
		/// <param name="position">The position in world space the object will be placed at.</param>
		/// <param name="rotation">The rotation in world space the object will be oriented to.</param>
		/// <param name="parent">The object that the newly returned retrievable will be childed to.</param>
		public static T Retrieve(Vector3 position, Quaternion rotation, Transform parent) {

			// retrieve an item from the pool.
			T poolable = RetrievePoolable();

			// apply our position, rotation, and parent.
			poolable.transform.position = position;
			poolable.transform.rotation = rotation;
			poolable.transform.parent = parent;

			//return our new object.
			return poolable;
		}

		/// <summary>
		/// Returns a <c>IPoolable</c> to the pool to be immediately disabled and then, at a later date, reused (via retrieval).
		/// </summary>
		/// <param name="poolable">The <c>IPoolable</c> to return to the pool and immediately disable.</param>
		public static void Return(T poolable) {

			PooledItem pooledItem = GetOrCreatePooledItemByPoolable(poolable);

			// flag the pooled item as pooled and retrievable.
			pooledItem.IsPooled      =
			pooledItem.IsRetrievable = true;

			// disable the poolable and inform the poolable of having been returned.
			poolable.gameObject.SetActive(false);
			poolable.OnReturnedToPool();
		}

		/// <summary>
		/// Marks the given <c>IPoolable</c> as being able to be retrieved but doesn't attempt to pool and disable it.
		/// </summary>
		public static void MarkRetrievable(T poolable) {
			PooledItem pooledItem = GetOrCreatePooledItemByPoolable(poolable);
			pooledItem.IsRetrievable = true;
		}

        public static void ReturnAll() {

            foreach (PooledItem item in pooledItems) {
                item.IsPooled = item.IsRetrievable = true;

                item.Poolable.gameObject.SetActive(false);
                item.Poolable.OnReturnedToPool();
            }
        }

        //-----------------------------------------------------------------------------------------
        // Private Methods:
        //-----------------------------------------------------------------------------------------

        // ReSharper disable once RedundantTypeSpecificationInDefaultExpression
        private static PooledItem Create(T usingExistingPoolable = default(T)) {

			// default our poolable to an existing one, which may be default.
			T poolable = usingExistingPoolable;

			// if we haven't been given an existing poolable, create one.
			if (poolable == default(T)) {

				// instantiate a new poolable based on the prototype.
				poolable = Object.Instantiate(prototype);

				// ensure the poolable is positioned at the prototype's position and its game object is enabled.
				// N.B. we don't use the Instantiate position, rotation, etc. arguments because we want to use this opportunity to
				// cache the transform and game object at the level of the ApacheComponent. 
				poolable.transform.position = prototype.transform.position;
				poolable.transform.rotation = prototype.transform.rotation;

				// have the handler handle this poolable.
				handler.Handle(poolable);
			}
			
			// instantiate a pooled item for the poolable and add it to the collection, flagging it as unretrievable for now.
			PooledItem pooledItem = new PooledItem(poolable);
			pooledItems.Add(pooledItem);

			return pooledItem;
		}

		private static void Prewarm(int num) {
			for (int i = 0; i < num; i++) {

				// create a pooled item.
				PooledItem pooledItem = Create();

				// flag the pooled item as pooled and retrievable and inform the poolable of having been returned.
				pooledItem.IsPooled      =
				pooledItem.IsRetrievable = true;
				pooledItem.Poolable.gameObject.SetActive(false);
				pooledItem.Poolable.OnReturnedToPool();
			}
		}

		private static PooledItem GetOrCreatePooledItemByPoolable(T poolable) {

			// find the corresponding pooled item.
			foreach (PooledItem item in pooledItems) {
				if (item.Poolable != poolable) continue;
				return item;
			}

			// we didn't find the corresponding item, so create one.
			return Create(poolable);
		}

		private static T RetrievePoolable() {

			// ReSharper disable once RedundantTypeSpecificationInDefaultExpression
			PooledItem pooledItem = default(PooledItem);

			// iterate over items looking for something retrievable.
			foreach (PooledItem item in pooledItems) {

				// if we find a pooled item, set it and back out, as that's the ideal case.
				if (item.IsPooled) {
					pooledItem = item;
					break;
				}

				// if we find a retrievable item, set it.
				// N.B. no item here will be pooled.
				if (item.IsRetrievable) {
					pooledItem = item;
				}

				// N.B. even if we have something, we don't break out because we're hoping to find a truly pooled and not merely retrievable
				// item on the next loop.
			}

			// if we make it here with a retrievable item which is not pooled...
			if (pooledItem != default(PooledItem) && !pooledItem.IsPooled) {

				// in order to support an init/deinit style approach on retrieve/return, inform the poolable that it returned to the pool.
				pooledItem.Poolable.OnReturnedToPool();
			}

			// we didn't find an existing pooled or retrievable item...
			else if (pooledItem == default(PooledItem)) {

				// if we're not dynamic, return default, as we can't create new poolables.
				// ReSharper disable once RedundantTypeSpecificationInDefaultExpression
				if (!isDynamic) return default;

				// create a new pooled item.
				pooledItem = Create();
			}

			// flag the pooled item as no longer pooled and retrievable.
			pooledItem.IsPooled      =
			pooledItem.IsRetrievable = false;

			// enable the poolable and inform the poolable of having been retrieved, then return the poolable.
			pooledItem.Poolable.gameObject.SetActive(true);
			pooledItem.Poolable.OnRetrievedFromPool();
			return pooledItem.Poolable;
		}

		//-----------------------------------------------------------------------------------------
		// Classes:
		//-----------------------------------------------------------------------------------------

		public class PooledItem {

			//-----------------------------------------------------------------------------------------
			// Public Properties:
			//-----------------------------------------------------------------------------------------

			/// <summary>
			/// The <c>IPoolable</c> which is managed by the pool.
			/// </summary>
			public T Poolable { get; set; }

			/// <summary>
			/// Is the <c>IPoolable</c> currently sitting disabled within the pool.
			/// </summary>
			public bool IsPooled { get; set; }

			/// <summary>
			/// Is the <c>IPoolable</c> not in use and available to be released from the pool.
			/// </summary>
			/// <remarks>
			/// It's possible that an <c>IPoolable</c> has not been returned to the pool but is still retrievable.
			/// </remarks>
			public bool IsRetrievable { get; set; } = true;

			//-----------------------------------------------------------------------------------------
			// Public Methods:
			//-----------------------------------------------------------------------------------------

			public PooledItem(T poolable) {
				Poolable = poolable;
			}
		}
	}
}