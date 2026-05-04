using TMPro;
using UnityEngine;

public class DrillingView : GuiView {

	//-----------------------------------------------------------------------------------------
	// Inspector Variables:
	//-----------------------------------------------------------------------------------------

	[SerializeField] protected TextMeshProUGUI depthValueText;

	//-----------------------------------------------------------------------------------------
	// Public Methods:
	//-----------------------------------------------------------------------------------------

	public void SetDepthValue(int depthValue) {
		depthValueText.text = depthValue.ToString(); 
	}
}