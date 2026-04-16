using Apache.Core;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class AudioHandler : ComponentSingletonProtected<AudioHandler> {

	//-----------------------------------------------------------------------------------------
	// Inspector Variables:
	//-----------------------------------------------------------------------------------------

	[SerializeField] protected AudioClip buttonDownClip;
	[SerializeField] protected AudioClip buttonUpClip; 
	[SerializeField] protected AudioClip buttonHoveredClip;
	[SerializeField] protected AudioClip buttonUnhoveredClip;

	//-----------------------------------------------------------------------------------------
	// Public Methods:
	//-----------------------------------------------------------------------------------------

	public static void PlayButtonDownSfx() {
		AudioSource.PlayClipAtPoint(Instance.buttonDownClip, Vector3.zero);
	}

	public static void PlayButtonUpSfx() {
		AudioSource.PlayClipAtPoint(Instance.buttonUpClip, Vector3.zero);
	}

	public static void PlayButtonHoveredSfx() {
		AudioSource.PlayClipAtPoint(Instance.buttonHoveredClip, Vector3.zero);
	}

	public static void PlayButtonUnhoveredSfx() {
		AudioSource.PlayClipAtPoint(Instance.buttonUnhoveredClip, Vector3.zero);
	}
}