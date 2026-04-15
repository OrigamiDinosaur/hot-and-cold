using System;
using System.Collections.Generic;
using UnityEngine;

namespace Apache.Core {

	/// <summary>
	/// Provides caching of <c>WaitForSeconds</c>, <c>WaitForSecondsRealtimeCacheable</c> (different from <c>WaitForSecondsRealtime</c>),
	/// <c>WaitForEndOfFrame</c>, and <c>WaitForFixedUpdate</c> objects over the lifetime of an application. Each time a 
	/// <c>WaitForSeconds</c> or similar object is allocated, 21 bytes become garbage almost instantly. Over time, this can add up and
	/// result in unnecessarily frequent GC collections. This class helps with caching <c>WaitForSeconds</c>, and keeps those bytes in
	/// memory so they don't contribute towards collections.
	/// 
	/// This class maintains its own <c>WaitForSecondsRealtimeCacheable</c> yield instruction, which is a custom implementation of
	/// <c>WaitForSecondsRealtime</c> which supports reuse and therefore caching.
	/// </summary>
	public static class Wait {

		//-----------------------------------------------------------------------------------------
		// Private Constants:
		//-----------------------------------------------------------------------------------------

		private const float PREWARM_WAIT_CACHE_TIME_DELTA   = 0.25f; // the difference between waits automatically cached on init.
		private const float PREWARM_WAIT_CACHE_MAX_DURATION = 10;    // the max duration of waits automatically cached on init.

		private const int PREWARM_WAIT_REALTIME_CACHE_NUM = 3; // the number of realtime objects to automatically cache on init.

		//-----------------------------------------------------------------------------------------
		// Backing Fields:
		//-----------------------------------------------------------------------------------------

		private static WaitForEndOfFrame  _endOfFrame;
		private static WaitForFixedUpdate _fixedUpdate;

		//-----------------------------------------------------------------------------------------
		// Private Fields:
		//-----------------------------------------------------------------------------------------

		private static readonly Dictionary<float, WaitForSeconds> waitCache = new Dictionary<float, WaitForSeconds>();

		private static readonly HashSet<WaitForSecondsRealtimeCacheable> waitRealtimeCache = new HashSet<WaitForSecondsRealtimeCacheable>();

		//-----------------------------------------------------------------------------------------
		// Public Properties:
		//-----------------------------------------------------------------------------------------

		public static WaitForEndOfFrame  EndOfFrame  => _endOfFrame  ?? (_endOfFrame  = new WaitForEndOfFrame ());
		public static WaitForFixedUpdate FixedUpdate => _fixedUpdate ?? (_fixedUpdate = new WaitForFixedUpdate());

		//-----------------------------------------------------------------------------------------
		// Public Methods:
		//-----------------------------------------------------------------------------------------

		/// <summary>
		/// Gets a cached (default) or uncached <c>WaitForSeconds</c> object corresponding to the given time in seconds, scaled by time scale.
		/// </summary>
		/// <param name="seconds">The duration to wait in seconds, scaled by time scale.</param>
		/// <param name="shouldCache">Whether to cache or retrieve from cache the requested object. Defaults to true.</param>
		/// <returns>The <c>WaitForSeconds</c> object.</returns>
		public static WaitForSeconds Seconds(float seconds, bool shouldCache = true) {

			if (!shouldCache) return new WaitForSeconds(seconds);

			// if the wait key is not cached, cache it.
			if (!waitCache.ContainsKey(seconds)) {
				waitCache.Add(seconds, new WaitForSeconds(seconds));
			}

			return waitCache[seconds];
		}

		/// <summary>
		/// Gets a cached (default) or uncached <c>CustomYieldInstruction</c> object corresponding to the given time in realtime seconds
		/// unaffected by time scale.
		/// </summary>
		/// <param name="seconds">The duration to wait in realtime seconds, unaffected by time scale.</param>
		/// <param name="shouldCache">Whether to cache or retrieve from cache the requested object. Defaults to true.</param>
		/// <returns>
		/// The <c>CustomYieldInstruction</c> object which is either a <c>WaitForSecondsRealtime</c> if not we're caching or a
		/// <c>WaitForSecondsRealtimeCacheable</c> if we are.</returns>
		/// <remarks>
		/// It is best not to attempt to cache an instance of <c>WaitForSecondsRealtimeCacheable</c> (which will be the return type under
		/// the hood if caching) manually and outside of <c>Wait</c>.cs, for special considerations such as checking whether an instance
		/// is currently in use and then resetting it are required for using it effectively.
		/// </remarks>
		public static CustomYieldInstruction SecondsRealtime(float seconds, bool shouldCache = true) {

			// if we're not caching, just return a standard WaitForSecondsRealtime.
			if (!shouldCache) return new WaitForSecondsRealtime(seconds);
			
			// loop through the cache to find an object not in use.
			WaitForSecondsRealtimeCacheable wait = null;
			foreach (WaitForSecondsRealtimeCacheable waitRealtime in waitRealtimeCache) {
				if (waitRealtime.IsInUse) continue;
				wait = waitRealtime;
				break;
			}

			// if we didn't find one in the cache, instantiate and add it now.
			if (wait == null) {
				wait = new WaitForSecondsRealtimeCacheable();
				waitRealtimeCache.Add(wait);
			}

			// prepare the wait as in use with the given seconds.
			wait.Use(seconds);
			
			return wait;
		}

		//-----------------------------------------------------------------------------------------
		// Private Methods:
		//-----------------------------------------------------------------------------------------

		// ReSharper disable once UnusedMember.Local

		[RuntimeInitializeOnLoadMethod]
		private static void Init() {
			
			// prewarm the cache with a number of common wait objects so we can avoid instantiating and adding to the dictionary unless we have a more specific time requirement.
			for (float i = PREWARM_WAIT_CACHE_TIME_DELTA; i < PREWARM_WAIT_CACHE_MAX_DURATION; i += PREWARM_WAIT_CACHE_TIME_DELTA) {

				// N.B. we check we don't already have a cached value for this, as this init may be called after other objects' Awakes.
				if (!waitCache.ContainsKey(i)) {
					waitCache.Add(i, new WaitForSeconds(i));
				}
			}

			// prewarm the realtime wait cache with the required number of objects.
			// N.B. we use while as the cache may already have objects in it since RuntimeInitializeOnLoadMethod methods run after Awake.
			while (waitRealtimeCache.Count < PREWARM_WAIT_REALTIME_CACHE_NUM) {
				waitRealtimeCache.Add(new WaitForSecondsRealtimeCacheable());
			}
		}

		//-----------------------------------------------------------------------------------------
		// Classes:
		//-----------------------------------------------------------------------------------------

		/// <inheritdoc />
		/// <summary>
		/// A custom implementation of <c>WaitForSecondsRealtime</c> which can be reused and therefore cached.
		/// </summary>
		/// <remarks>
		/// It is best not to attempt to cache an instance of <c>WaitForSecondsRealtimeCacheable</c> manually/outside of <c>Wait</c>.cs, for special
		/// considerations such as checking whether an instance is currently in use and then resetting it are required for using it effectively.
		/// </remarks>
		private class WaitForSecondsRealtimeCacheable : CustomYieldInstruction {

			//-----------------------------------------------------------------------------------------
			// Private Fields:
			//-----------------------------------------------------------------------------------------

			private float startTime;
			private float duration;

			//-----------------------------------------------------------------------------------------
			// Public Properties:
			//-----------------------------------------------------------------------------------------

			public bool IsInUse { get; private set; }

			//-----------------------------------------------------------------------------------------
			// Public Properties:
			//-----------------------------------------------------------------------------------------

			public override bool keepWaiting {
				get {

					// throw invalid op if this property is evaluated while not in use.
					// N.B. the assumption is that only a Unity coroutine will internally use this property, and therefore it shouldn't know about this yield
					// instruction if it has not been set up to be in use.
					// N.B. this also safeguards against improper external usage whereby a developer may attempt to cache this object manually (as a 
					// CustomYieldInstruction) but not know about checking whether it is in use and preparing it for use correctly.
					if (!IsInUse) throw new InvalidOperationException();

					bool shouldKeepWaiting = (Time.realtimeSinceStartup - startTime) < duration;

					// when we find we should no longer keep waiting, flag no longer in use.
					// N.B. we do this work in this property because it is the only way we can be sure it is being yielded over and evaluated by a Unity coroutine.
					if (!shouldKeepWaiting) {
						IsInUse = false;
					}

					return shouldKeepWaiting;
				}
			}

			//-----------------------------------------------------------------------------------------
			// Public Methods:
			//-----------------------------------------------------------------------------------------

			/// <summary>
			/// Use or reuse this yield instruction with a new duration.
			/// </summary>
			/// <param name="aDuration"></param>
			public void Use(float aDuration) {

				// if we're already in use, throw invalid op, for, if not careful, reusing this yield instruction may cause other coroutines currently
				// yielding over it to continue waiting in unexpected and error-prone ways.
				if (IsInUse) throw new InvalidOperationException();

				startTime = Time.realtimeSinceStartup;
				duration = aDuration;
				IsInUse = true;
			}
		}
	}
}