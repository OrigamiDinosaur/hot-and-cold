using TMPro;
using UnityEngine;

[System.Serializable]
public class TextOptions {

	public FontStyles fontStyle;
	public float fontSize;
	public Color color; 
}

public class TextButton : MonoBehaviour {

	//-----------------------------------------------------------------------------------------
	// Inspector Variables:
	//-----------------------------------------------------------------------------------------

	[Header("References")]

	[SerializeField] protected OriButton button; 
	[SerializeField] protected TextMeshProUGUI text;

	[Header("Options")]

	[SerializeField] protected TextOptions normalStyle;
	[SerializeField] protected TextOptions hoveredStyle;
	[SerializeField] protected TextOptions pressedStyle;

	
	//-----------------------------------------------------------------------------------------
	// Private Fields:
	//-----------------------------------------------------------------------------------------

	private bool isHovered;

	//-----------------------------------------------------------------------------------------
	// Unity Lifecycle:
	//-----------------------------------------------------------------------------------------

	protected void OnEnable() {
		button.PointerEntered += Button_PointerEntered;
		button.PointerExited += Button_PointerExited;
		button.PointerDown += Button_PointerDown;
		button.PointerUp += Button_PointerUp; 
	}

	protected void Start() {
		button.image.alphaHitTestMinimumThreshold = 0.0f;
	}

	protected void OnDisable() {
		button.PointerEntered -= Button_PointerEntered;
		button.PointerExited -= Button_PointerExited;
		button.PointerDown -= Button_PointerDown;
		button.PointerUp -= Button_PointerUp;
	}

	//-----------------------------------------------------------------------------------------
	// Event Handlers:
	//-----------------------------------------------------------------------------------------

	private void Button_PointerEntered() {
		isHovered = true;

		SetStyle(hoveredStyle);
	}

	private void Button_PointerExited() {
		isHovered = false;

		SetStyle(normalStyle);
	}

	private void Button_PointerDown() {
		SetStyle(pressedStyle);
	}

	private void Button_PointerUp() {
		SetStyle(isHovered ? hoveredStyle : pressedStyle);
	}

	//-----------------------------------------------------------------------------------------
	// Private Methods:
	//-----------------------------------------------------------------------------------------

	private void SetStyle(TextOptions style) {

		text.fontStyle = style.fontStyle;
		text.fontSize = style.fontSize;
		text.color = style.color; 
	}
}