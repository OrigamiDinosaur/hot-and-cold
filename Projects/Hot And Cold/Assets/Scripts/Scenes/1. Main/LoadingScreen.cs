using Apache.Core;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : ApacheComponent {

	//-----------------------------------------------------------------------------------------
	// Inspector Variables:
	//-----------------------------------------------------------------------------------------

	[SerializeField] protected Image progressBar;

	//-----------------------------------------------------------------------------------------
	// Public Methods:
	//-----------------------------------------------------------------------------------------

	public void UpdateProgress(float normalizedProgress) {
		progressBar.fillAmount = normalizedProgress; 
	}
}