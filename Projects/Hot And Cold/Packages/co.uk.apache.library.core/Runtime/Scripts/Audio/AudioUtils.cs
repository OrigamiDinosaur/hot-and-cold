using UnityEngine;

namespace Apache.Core {
	public static class AudioUtils {

		//-----------------------------------------------------------------------------------------
		// Constants:
		//-----------------------------------------------------------------------------------------

		public static readonly FloatRange DEFAULT_VOLUME_RANGE = new FloatRange(0.9f, 1f);

		public static readonly FloatRange DEFAULT_PITCH_RANGE  = new FloatRange(0.85f, 1.05f);

		//-----------------------------------------------------------------------------------------
		// Public Methods:
		//-----------------------------------------------------------------------------------------

		/// <summary>
		/// Returns a random audio clip from an array of clips.
		/// </summary>
		/// <param name="clips">The clips from which to return a random one.</param>
		/// <returns>A random clip within the given <c>clips</c> array</returns>
		public static AudioClip Random(AudioClip[] clips) {
			return clips[UnityEngine.Random.Range(0, clips.Length)];
		}

		/// <summary>
		/// Configures an audio source with a random clip and pitch.
		/// </summary>
		/// <param name="source">The audio source to configure.</param>
		/// <param name="clips">The clips from which to choose a random one.</param>
		/// <param name="pitch">The pitch range within which to choose at random.</param>
		public static void ConfigRandom(AudioSource source, AudioClip[] clips, FloatRange pitch) {
			source.clip = Random(clips);
			source.pitch = pitch.Random();
		}

		/// <summary>
		/// Configures and plays an audio source with a random clip and pitch.
		/// </summary>
		/// <param name="source">The audio source to configure and play.</param>
		/// <param name="clips">The clips from which to choose a random one.</param>
		/// <param name="pitch">The pitch range within which to choose at random.</param>
		/// <param name="delay">The delay in seconds before playing.</param>
		public static void PlayRandom(AudioSource source, AudioClip[] clips, FloatRange pitch, float delay = 0) {

			if (!HasSourceAndClips(source, clips)) return;

			ConfigRandom(source, clips, pitch);

			if (delay > 0) {
				source.PlayDelayed(delay);
			}
			else {
				source.Play();
			}
		}

		/// <summary>
		/// Returns whether the given <c>AudioSource</c> is not null and <c>AudioClip</c> array contains at least one clip.
		/// </summary>
		/// <param name="source">The <c>AudioSource</c> to check for not null.</param>
		/// <param name="clips">The <c>AudioClip</c> array to check that it contains at least one clip.</param>
		/// <returns></returns>
		public static bool HasSourceAndClips(AudioSource source, AudioClip[] clips) {
			return (source != null && clips != null && clips.Length > 0);
		}
	}
}