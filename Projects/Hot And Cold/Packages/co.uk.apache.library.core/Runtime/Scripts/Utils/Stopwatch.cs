using Debug = UnityEngine.Debug;

namespace Apache.Core {
	public static class Stopwatch {

		//-----------------------------------------------------------------------------------------
		// Constants:
		//-----------------------------------------------------------------------------------------

		private const float MILLISECONDS_FREQUENCY_DIVIDEND = 1000;
		private const float MICROSECONDS_FREQUENCY_DIVIDEND = MILLISECONDS_FREQUENCY_DIVIDEND * 1000;

		private const string LOG_FORMAT = "{0} = {1} {2}";

		//-----------------------------------------------------------------------------------------
		// Private Variables:
		//-----------------------------------------------------------------------------------------

		private static string label;
		private static bool shouldShowMicroseconds;
		private static readonly System.Diagnostics.Stopwatch stopwatch;

		private static readonly float millisecondsPerTick;
		private static readonly float microsecondsPerTick;

		//-----------------------------------------------------------------------------------------
		// Public Methods:
		//-----------------------------------------------------------------------------------------

		static Stopwatch() {
			stopwatch = new System.Diagnostics.Stopwatch();

			// work out ticks per duration.
			millisecondsPerTick = MILLISECONDS_FREQUENCY_DIVIDEND / System.Diagnostics.Stopwatch.Frequency;
			microsecondsPerTick = MICROSECONDS_FREQUENCY_DIVIDEND / System.Diagnostics.Stopwatch.Frequency;
		}

		/// <summary>Starts a stopwatch, stopping and restarting the stopwatch if it is already running.</summary>
		/// <param name="label">A descriptive label which will be used for identification when logging elapsed time.</param>
		/// <param name="shouldShowMicroseconds">Whether we should log microseconds or milliseconds (default).</param>
		// ReSharper disable ParameterHidesMember
		public static void Start(string label, bool shouldShowMicroseconds = false) {
		// ReSharper restore ParameterHidesMember

			// call stop if the stopwatch is already running.
			if (stopwatch.IsRunning) {
				Stop();
			}

			// set label, whether we show ticks and start the stopwatch.
			Stopwatch.label = label;
			Stopwatch.shouldShowMicroseconds = shouldShowMicroseconds;
			stopwatch.Start();
		}

		/// <summary>Stops the stopwatch and logs the elapsed microseconds or milliseconds.</summary>
		public static void Stop() {
			stopwatch.Stop();
			float numberOfElapsedUnits = stopwatch.ElapsedTicks * ((shouldShowMicroseconds) ? microsecondsPerTick : millisecondsPerTick);
			Debug.Log(string.Format(LOG_FORMAT, label, numberOfElapsedUnits, (shouldShowMicroseconds) ? "μs" : "ms"));
			stopwatch.Reset();
		}
	}
}