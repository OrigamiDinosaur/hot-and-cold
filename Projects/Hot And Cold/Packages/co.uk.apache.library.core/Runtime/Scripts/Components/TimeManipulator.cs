using UnityEngine;

namespace Apache.Core {
	public class TimeManipulator : ApacheComponent {

		//-----------------------------------------------------------------------------------------
		// Constants:
		//-----------------------------------------------------------------------------------------

		private const float NORMAL_TIME_SCALE_DEFAULT = 1;
		private const float FAST_FORWARD_TIME_SCALE_DEFAULT = 7;
		private const float INCREASE_TIME_SCALE_MULTIPLIER_DEFAULT = 1.5f;
		private const float DECREASE_TIME_SCALE_MULTIPLIER_DEFAULT = 0.5f;

		private const float SKIP_TIME_SCALE = 24f;

		//-----------------------------------------------------------------------------------------
		// Inspector Variables:
		//-----------------------------------------------------------------------------------------

		[ApacheSpace]

		[SerializeField] protected float normalTimeScale = NORMAL_TIME_SCALE_DEFAULT;
		[SerializeField] protected float fastForwardTimeScale = FAST_FORWARD_TIME_SCALE_DEFAULT;
		[SerializeField] protected float increaseTimeScaleMultiplier = INCREASE_TIME_SCALE_MULTIPLIER_DEFAULT;
		[SerializeField] protected float decreaseTimeScaleMultiplier = DECREASE_TIME_SCALE_MULTIPLIER_DEFAULT;

		[ApacheSpace]

		[SerializeField] protected bool skipOnStart;
		[SerializeField] protected float skipTime;

		//-----------------------------------------------------------------------------------------
		// Private Fields:
		//-----------------------------------------------------------------------------------------

		private static TimeManipulator instance;

		private static float defaultFixedDeltaTime = float.NaN;

		//-----------------------------------------------------------------------------------------
		// Public Properties:
		//-----------------------------------------------------------------------------------------

		public static float TimeScale => Time.timeScale;

		public static bool IsSkipping { get; private set; }

		//-----------------------------------------------------------------------------------------
		// Unity Lifecycle:
		//-----------------------------------------------------------------------------------------

		protected void Awake() {
			instance = this;

			defaultFixedDeltaTime = Time.fixedDeltaTime;

			// if we're skipping on start...
			if (skipOnStart) {

				// flag we're skipping and set time scale to skip time scale.
				IsSkipping = true;
				SetTimeScale(SKIP_TIME_SCALE);

				// after the delay, reset normal time skipping.
				InvokeAction(skipTime, () => {
					SetTimeScale(normalTimeScale);
					IsSkipping = false;
				});
				return;
			}

			// if we made it here, just set normal time scale.
			Time.timeScale = normalTimeScale;
		}

		protected void Update() {
			UpdateShortcuts();
		}

		//-----------------------------------------------------------------------------------------
		// Public Methods:
		//-----------------------------------------------------------------------------------------

		// Provides useful shortcuts for speeding up time.
		// N.B. this is also designed to be called from other scripts.
		public static void UpdateShortcuts() {

			bool shouldUseDefaults = (instance == null);

			if (Input.GetKeyDown(KeyCode.UpArrow)) {
				SetTimeScale((shouldUseDefaults) ? FAST_FORWARD_TIME_SCALE_DEFAULT : instance.fastForwardTimeScale);
			}
			else if (Input.GetKeyDown(KeyCode.DownArrow)) {
				SetTimeScale(NORMAL_TIME_SCALE_DEFAULT);
			}
			else if (Input.GetKeyDown(KeyCode.RightArrow)) {
				SetTimeScale(Time.timeScale * ((shouldUseDefaults) ? INCREASE_TIME_SCALE_MULTIPLIER_DEFAULT : instance.increaseTimeScaleMultiplier));
			}
			else if (Input.GetKeyDown(KeyCode.LeftArrow)) {
				SetTimeScale(Time.timeScale * ((shouldUseDefaults) ? DECREASE_TIME_SCALE_MULTIPLIER_DEFAULT : instance.decreaseTimeScaleMultiplier));
			}
		}

		public static void SetTimeScale(float timeScale) {

			// if we don't have a value for fixed delta time, grab it and make it default.
			if (float.IsNaN(defaultFixedDeltaTime)) {
				defaultFixedDeltaTime = Time.fixedDeltaTime;
			}

			// set time scale.
			Time.timeScale = timeScale;

			// set fixed delta time, making sure not to set it zero as this causes some bad physics bugs.
			// N.B. FixedUpdate is not called anyway when time scale is zero.
			if (timeScale > 0) {
				Time.fixedDeltaTime = defaultFixedDeltaTime * timeScale;
			}
		}
	}
}