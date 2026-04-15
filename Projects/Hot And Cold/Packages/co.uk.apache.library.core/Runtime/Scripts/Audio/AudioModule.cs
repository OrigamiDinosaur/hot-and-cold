using System;
using UnityEngine;

namespace Apache.Core {
	public class AudioModule : ApacheComponent {

		//-----------------------------------------------------------------------------------------
		// Constants:
		//-----------------------------------------------------------------------------------------

		private const float DEFAULT_VOLUME_BY_TIME_RANGE_MIN = 0.5f;

		//-----------------------------------------------------------------------------------------
		// Type Definitions:
		//-----------------------------------------------------------------------------------------

		public enum ClipSelectionModes {
			Random,
			Sequential
		}

		//-----------------------------------------------------------------------------------------
		// Inspector Variables:
		//-----------------------------------------------------------------------------------------

		[Header("References")]
		
		// N.B. we require a source because this component was designed to work with an audio source on the same game object.
		// N.B. we use ref requiring instead of RequireComponent because the former adds components below as opposed to above the script.
		[ComponentRef(true), SerializeField] protected AudioSource source;

		[SerializeField] protected AudioClip[] clips;

		[ApacheSpace]

		[SerializeField] protected ClipSelectionModes clipSelectionMode;

		[Header("Volume")]

		[FloatRangeSlider]
		[Tooltip("The range of possible volumes. If random volume is enabled, a value between min and max will be chosen at random. " +
					"Alternatively, this volume range is used by the SetVolumeT method offering support for programmatic lerping between volumes.")]
		[SerializeField] protected FloatRange volumeRange = AudioUtils.DEFAULT_VOLUME_RANGE;

		[InspectorLabel("Random Volume")]
		[SerializeField] protected bool shouldUseRandomVolume = true;

		[ApacheSpace]

		[InspectorLabelBool]
		[SerializeField] protected bool shouldScaleVolumeByTime;
		
		[FloatRangeSlider]
		[Tooltip("The volume magnitude by time, where min corresponds to the slowest, most-scaled-down time, and max corresponds to normal time.")]
		[SerializeField] protected FloatRange volumeByTimeRange = new FloatRange(DEFAULT_VOLUME_BY_TIME_RANGE_MIN, 1);
		
		[Header("Pitch")]

		[FloatRangeSlider(0, 2)]
		[Tooltip("The range of possible pitches. If random pitch is enabled, a value between min and max will be chosen at random. " +
					"Alternatively, this pitch range is used by the SetPitchT method offering support for programmatic lerping between pitches.")]
		[SerializeField] protected FloatRange pitchRange = AudioUtils.DEFAULT_PITCH_RANGE;

		[InspectorLabel("Random Pitch")]
		[SerializeField] protected bool shouldUseRandomPitch = true;
		
		[ApacheSpace]

		[InspectorLabelBool]
		[SerializeField] protected bool shouldScalePitchByTime;

		[Tooltip("The magnitude by which pitch scales with time. Make this number smaller to have a lesser effect on pitch.")]
		[SerializeField] protected float pitchByTimeScaleMagnitude = 1;

		[Header("Delay")]

		[SerializeField] protected float delay;

		[Header("Options")]

		[InspectorLabelBool]
		[SerializeField] protected bool shouldPlayOnAwake;

		[InspectorLabelBool]
		[SerializeField] protected bool shouldPlayOnEnable;

		[ApacheSpace]

		[InspectorLabel("Don't Replay If Playing")]
		[Tooltip("If the AudioSource is already playing, should we *not* try to replay when Play is called on this module?")]
		[SerializeField] protected bool dontReplayIfPlaying;
		
		//-----------------------------------------------------------------------------------------
		// Backing Fields:
		//-----------------------------------------------------------------------------------------

		/// <summary>The default starting pitch of the audio source.</summary>
		protected float _defaultPitch;

		/// <summary>The target pitch we're aiming for, which may be scaled based on time.</summary>
		protected float _targetPitch;

		/// <summary>The default starting volume of the audio source.</summary>
		protected float _defaultVolume;

		/// <summary>The target volume we're aiming for, which may be scaled based on time.</summary>
		protected float _targetVolume;

		//-----------------------------------------------------------------------------------------
		// Protected Fields:
		//-----------------------------------------------------------------------------------------

		/// <summary>The previously randomly selected clip, persisted to ensure we choose a different one next time.</summary>
		protected AudioClip prevSelectedClip;
		
		protected bool shouldPlayScheduled;
		protected double scheduledAbsoluteDspTime;

		protected int sequentialClipId;

		//-----------------------------------------------------------------------------------------
		// Public Properties:
		//-----------------------------------------------------------------------------------------

		/// <summary>Is the audio source currently playing?</summary>
		public virtual bool IsPlaying => (source != null && source.isPlaying);

		/// <summary>Does this module have one or more <c>AudioClip</c>s to play?</summary>
		public bool HasClip => (clips != null && clips.Length >= 1);

		/// <summary>The clip (or first of many clips) of the audio module.</summary>
		/// <remarks>If this property is used, it is assumed that only a single audio clip is being used.</remarks>
		public virtual AudioClip Clip {
			get {
				if (clips == null || clips.Length == 0) return null;
				return clips[0];
			}
		}

		/// <summary>What is the length of the audio module's clip?</summary>
		/// <remarks>If this property is used, it is assumed that only a single audio clip is being used.</remarks>
		public virtual float Length {
			get {
				if (clips == null || clips.Length == 0) return 0;
				return clips[0].length;
			}
		}

		/// <summary>What is the length of the audio module's clip as a double suitable for absolute DSP timings?</summary>
		/// <remarks>If this property is used, it is assumed that only a single audio clip is being used.</remarks>
		public virtual double LengthDsp {
			get {
				if (clips == null || clips.Length == 0) return 0;
				return Clip.samples / (double)Clip.frequency;
			}
		}

		//-----------------------------------------------------------------------------------------
		// Protected Properties:
		//-----------------------------------------------------------------------------------------

		/// <summary>Whether this module is currently able to play new clips.</summary>
		protected bool CanPlay => (AudioUtils.HasSourceAndClips(source, clips) && !(source.isPlaying && dontReplayIfPlaying));

		/// <summary>The current pitch determined by our target pitch and either scaled or unscaled by time, depending on configuration.</summary>
		protected float CurrentPitch {
			get {
				if (!shouldScalePitchByTime) return _targetPitch;
				float timeScaleDelta = (1 - Time.timeScale) * pitchByTimeScaleMagnitude;
				return _targetPitch * (1 - timeScaleDelta);
			}
		}

		/// <summary>The current volume determined by our target volume and either scaled or unscaled by time, depending on configuration.</summary>
		/// <remarks>When time is scaled down, we want volume to approach min, but we don't want it to scale beyond max, so we clamp.</remarks>
		protected float CurrentVolume => _targetVolume * ((shouldScaleVolumeByTime) ? volumeByTimeRange.Lerp(Mathf.Clamp01(Time.timeScale)) : 1);

		//-----------------------------------------------------------------------------------------
		// Unity Lifecycle:
		//-----------------------------------------------------------------------------------------

		protected void Awake() {

			// assign clip to the source right away if we have one.
			// N.B. this is for perhaps superstitious reasons that Unity can prepare the clip in some way if it's assigned to its audio source.
			if (HasClip && clips.Length == 1) {
				source.clip = clips[0];

				// now assign this as our previously selected clip.
				// N.B. this is in the eventuality that the module gains more clips in the future and we want to avoid replaying this one.
				prevSelectedClip = source.clip;
			}

			// grab default pitch and set that to our target pitch.
			_defaultPitch =
			_targetPitch  = source.pitch;

			// grab default volume and set that to our target volume.
			_defaultVolume =
			_targetVolume  = source.volume;

			// play on awake if necessary.
			if (shouldPlayOnAwake) {
				Play();
			}
		}

		protected void OnEnable() {
			Updater.Updated += _Update;

			// play on enable if necessary.
			if (shouldPlayOnEnable) {
				Play();
			}
		}

		// N.B. this is invoked on Unity's Update, but handled as an event for performance reasons.
		protected void _Update() {

			// if scaling pitch by time and the source is playing and its pitch doesn't match our current, update each frame.
			if (shouldScalePitchByTime  && source.isPlaying && source.pitch  != CurrentPitch) {
				UpdateSourcePitch();
			}

			// if scaling volume by time and the soruce is playing and its volume doesn't match our current, update each frame.
			if (shouldScaleVolumeByTime && source.isPlaying && source.volume != CurrentVolume) {
				UpdateSourceVolume();
			}
		}

		protected void OnDisable() {
			Updater.Updated -= _Update;
		}
		
		//-----------------------------------------------------------------------------------------
		// Public Methods:
		//-----------------------------------------------------------------------------------------
		
		/// <summary>
		/// Plays this modules <c>AudioSource</c> based on the module's <c>clips</c>. Handles randomly assigning a clip, randomly assigning a pitch
		/// (if that option is enabled), and playing with or without a delay. In addition, if the clip cannot be replayed while playing, playing
		/// will be abandoned.
		/// </summary>
		public virtual void Play() {
			if (!CanPlay) return;

			// keep track of a clip selection, and if we have clips to work with...
			AudioClip clip = null;
			if (HasClip) {
				switch (clipSelectionMode) {

					// handle the random selection of clips.
					case ClipSelectionModes.Random:

						// if we only have one clip, use that.
						if (clips.Length == 1) {
							clip = clips[0];
						}

						// if we have three or more clips and a previous selected clip to avoid, find a random clip which is different to the previous one we played.
						// N.B. we don't avoid repetition with two clips because, say we have a "pee!" and a "pew!" SFX and we try to not to repeat them, we'd end up
						// with "pee!", "pew!", "pee!", "pew!", over and over again, which would actually sound less random than occasional, random repetition.
						else if (clips.Length >= 3 && prevSelectedClip != null) {
							do {
								clip = AudioUtils.Random(clips);
							}
							while (clip == prevSelectedClip);
						}

						// otherwise just use a random clip, not worrying about repetition.
						else {
							clip = AudioUtils.Random(clips);
						}
						break;

					// handle the sequential selection of clips.
					case ClipSelectionModes.Sequential:

						// reset ID if it overshoots the number of clips available.
						if (sequentialClipId >= clips.Length) {
							sequentialClipId = 0;
						}

						// !!!
						print("Played clip sequentially with ID " + sequentialClipId);

						// grab clip corresponding to ID and increment ID.
						clip = clips[sequentialClipId];
						++sequentialClipId;
						break;

					// if we make it here, we don't recognise this clip selection mode, so throw invalid op.
					default:
						throw new InvalidOperationException();
				}
			}

			// assign clip to source if different, and keep track of this selection so we can avoid it if we have other options in the future.
			if (source.clip != clip) {
				source.clip = clip;
			}
			prevSelectedClip = clip;

			// grab a new target pitch if modifying pitch randomly, otherwise just use existing, then update source pitch.
			_targetPitch  = (shouldUseRandomPitch)  ? pitchRange.Random()  : _targetPitch;
			UpdateSourcePitch();

			// grab a new target volume if modifying volume randomly, otherwise just use existing, then update source volume.
			_targetVolume = (shouldUseRandomVolume) ? volumeRange.Random() : _targetVolume;
			UpdateSourceVolume();

			// handle playing scheduled if necessary.
			if (shouldPlayScheduled) {
				source.PlayScheduled(scheduledAbsoluteDspTime);

				// reset scheduling-related fields and back out.
				shouldPlayScheduled = false;
				scheduledAbsoluteDspTime = double.NaN;
				return;
			}

			// handle playing with a delay.
			if (delay > 0) {
				source.PlayDelayed(delay);
				return;
			}

			// not scheduling nor delaying, so just play.
			source.Play();
		}

		/// <summary>
		/// Plays the audio module with the given delay.
		/// </summary>
		/// <see cref="Play" />
		public virtual void PlayDelayed(float aDelay) {
			delay = aDelay;
			Play();
		}

		/// <summary>
		/// Schedules playing of this audio module to begin at the given absolute audio DSP time.
		/// </summary>
		/// <see cref="Play" />
		public virtual void PlayScheduled(double absoluteDspTime) {
			shouldPlayScheduled = true;
			scheduledAbsoluteDspTime = absoluteDspTime;
			Play();
		}

		/// <summary>
		/// Pauses this module's <c>AudioSource</c>.
		/// </summary>
		public virtual void Pause() {
			if (source == null) return;
			source.Pause();
		}

		/// <summary>
		/// Stops this module's <c>AudioSource</c>.
		/// </summary>
		public virtual void Stop() {
			if (source == null || !source.isPlaying) return;
			source.Stop();
		}

		/// <summary>
		/// Sets the volume on this module's <c>AudioSource</c>.
		/// </summary>
		/// <param name="value">The desired volume.</param>
		public virtual void SetVolume(float value) {
			if (source == null) return;
			_targetVolume = value;
			UpdateSourceVolume();
		}

		/// <summary>
		/// Sets the volume on this module's <c>AudioSource</c> based on a given <c>t</c> value between min and max of <c>Volume Range</c> in the inspector.
		/// </summary>
		/// <param name="t">The <c>t</c> value between min and max of <c>Volume Range</c> in the inspector.</param>
		public virtual void SetVolumeT(float t) {
			if (source == null) return;
			_targetVolume = volumeRange.Lerp(t);
			UpdateSourceVolume();
		}

		/// <summary>
		/// Sets the pitch on this module's <c>AudioSource</c>.
		/// </summary>
		/// <param name="value">The desired pitch.</param>
		public virtual void SetPitch(float value) {
			if (source == null) return;
			_targetPitch = value;
			UpdateSourcePitch();
		}

		/// <summary>
		/// Sets the pitch on this module's <c>AudioSource</c> based on a given <c>t</c> value between min and max of <c>Pitch Range</c> in the inspector.
		/// </summary>
		/// <param name="t">The <c>t</c> value between min and max of <c>Pitch Range</c> in the inspector.</param>
		public virtual void SetPitchT(float t) {
			if (source == null) return;
			_targetPitch = pitchRange.Lerp(t);
			UpdateSourcePitch();
		}

		/// <summary>Resets sequential playing such that the first clip will be played next.</summary>
		public virtual void ResetSequentialPlayingFromBeginning() {
			sequentialClipId = 0;
		}

		//-----------------------------------------------------------------------------------------
		// Protected Methods:
		//-----------------------------------------------------------------------------------------

		/// <summary>
		/// Updates our <c>AudioSource</c>'s pitch based on our current, desired pitch, either scaled or unscaled by time, depending on configuration.
		/// </summary>
		protected void UpdateSourcePitch() {
			source.pitch = CurrentPitch;
		}

		/// <summary>
		/// Updates our <c>AudioSource</c>'s volume based on our current, desired volume, either scaled or unscaled by time, depending on configuration.
		/// </summary>
		protected void UpdateSourceVolume() {
			source.volume = CurrentVolume;
		}
	}
}