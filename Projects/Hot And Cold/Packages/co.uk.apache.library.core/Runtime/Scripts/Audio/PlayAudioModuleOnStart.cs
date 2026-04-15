using UnityEngine;

namespace Apache.Core {
	public class PlayAudioModuleOnStart : ApacheComponent {

		//-----------------------------------------------------------------------------------------
		// Inspector Variables:
		//-----------------------------------------------------------------------------------------

		[SerializeField] protected AudioModule audioModule;

		//-----------------------------------------------------------------------------------------
		// Public Methods:
		//-----------------------------------------------------------------------------------------

		public void Start() {
			audioModule.Play();
		}
	}
}