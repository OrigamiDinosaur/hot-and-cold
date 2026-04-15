using System;
using UnityEngine;

namespace Apache.Core {
	public class AudioModuleVolumeFade : AudioModule {

		//-----------------------------------------------------------------------------------------
		// Inspector Variables:
		//-----------------------------------------------------------------------------------------

		[Header("Volume Fade")]
		
		[SerializeField] protected VolumeFade volumeUp;
		[SerializeField] protected VolumeFade volumeDown;
		
		//-----------------------------------------------------------------------------------------
		// Public Methods:
		//-----------------------------------------------------------------------------------------

		/// <inheritdoc />
		public override void Play() {
			if (!CanPlay) return;

			// if there's no delay, we don't need to tween, so just set to default volume, play, and back out.
			if (volumeUp.Duration == 0) {
				SetVolume(_defaultVolume);
				base.Play();
				return;
			}

			sequence.Cancel();
			
			// because calling base.Play() may change volume, determine our starting volume so we can reinstate it.
			float startingVolume = (!source.isPlaying) ? 0 : _targetVolume;

			// base configs and starts playing the audio source.
			base.Play();

			// reinstate starting volume and update it.
			_targetVolume = startingVolume;
			UpdateSourceVolume();

			// create a tween for handling volume up to default volume.
			StartVolumeTween(_defaultVolume, volumeUp);
		}

		// N.B. playing with delays or scheduling is not currently supported with volume fades, which always begin in sync with calls to Play.

		/// <inheritdoc />
		public override void PlayDelayed(float aDelay) {
			throw new NotImplementedException();
		}

		/// <inheritdoc />
		public override void PlayScheduled(double absoluteDspTime) {
			throw new NotImplementedException();
		}

		/// <inheritdoc />
		public override void Stop() {
			if (source == null || !source.isPlaying) return;

			// if there's no delay, we don't need to tween, so just set volume, stop, and back out.
			if (volumeDown.Duration == 0) {
				SetVolume(0);
				base.Stop();
				return;
			}

			// N.B. we tween volume down so we deliberately don't call base as that would actually stop the audio source. We want it playing until we fade out.

			sequence.Cancel();

			// start tweening the volume down and call base stop on complete.
			StartVolumeTween(0, volumeDown,
				base.Stop);
		}

		//-----------------------------------------------------------------------------------------
		// Private Methods:
		//-----------------------------------------------------------------------------------------

		private void StartVolumeTween(float targetVolume, VolumeFade volumeFade, Action onComplete = null) {

			// define the tween separately so we can optionally add an on complete action.
			LTDescr tween = LeanTween.value(sequence.GameObject, SetVolume, _targetVolume, targetVolume, volumeFade.Duration)
					.setEase(volumeFade.Ease);

			// add the on complete action if need be.
			if (onComplete != null) {
				tween.setOnComplete(onComplete);
			}

			// tell the tween to ignore time scale if necessary.
			if (volumeFade.ShouldIgnoreTimescale) {
				tween.setIgnoreTimeScale(true);
			}

			// pass the tween off to the sequence for handling.
			sequence.Tween(tween);
		}
		
		//-----------------------------------------------------------------------------------------
		// Classes:
		//-----------------------------------------------------------------------------------------

		[Serializable]
		public class VolumeFade {

			//-----------------------------------------------------------------------------------------
			// Inspector Variables:
			//-----------------------------------------------------------------------------------------

			[SerializeField] protected float duration;
			[SerializeField] protected LeanTweenType ease;

			[InspectorLabelBool]
			[SerializeField] protected bool shouldIgnoreTimescale;

			//-----------------------------------------------------------------------------------------
			// Public Properties:
			//-----------------------------------------------------------------------------------------

			public float Duration => duration;
			public LeanTweenType Ease => ease;
			public bool ShouldIgnoreTimescale => shouldIgnoreTimescale;
		}
	}
}