using UnityEngine;

public class AudioHandler : ComponentSingleton<AudioHandler> {

	//-----------------------------------------------------------------------------------------
	// Inspector Variables:
	//-----------------------------------------------------------------------------------------

	[SerializeField] protected AudioSource buttonDownClip;
	[SerializeField] protected AudioSource buttonUpClip; 
	[SerializeField] protected AudioSource buttonHoveredClip;
	[SerializeField] protected AudioSource buttonUnhoveredClip;
	[SerializeField] protected AudioSource buttonDisabledClip; 

	//-----------------------------------------------------------------------------------------
	// Public Methods:
	//-----------------------------------------------------------------------------------------

	public static void PlayButtonDownSfx() {
		Instance.buttonDownClip.Play();
	}

	public static void PlayButtonUpSfx() {
		Instance.buttonUpClip.Play();
	}

	public static void PlayButtonHoveredSfx() {
		Instance.buttonHoveredClip.Play();
	}

	public static void PlayButtonUnhoveredSfx() {
		Instance.buttonUnhoveredClip.Play();
	}

	public static void PlayButtonDisabledSfx() {
		Instance.buttonDisabledClip.Play();
	}
}