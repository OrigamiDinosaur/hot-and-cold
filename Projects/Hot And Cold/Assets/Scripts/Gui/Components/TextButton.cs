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
	[SerializeField] protected TextOptions disabledStyle; 

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
		button.InteractibilityChanged += Button_InteractabilityChanged; 
	}

	protected void Start() {
		button.image.alphaHitTestMinimumThreshold = 0.0f;
	}

	protected void OnDisable() {
		button.PointerEntered -= Button_PointerEntered;
		button.PointerExited -= Button_PointerExited;
		button.PointerDown -= Button_PointerDown;
		button.PointerUp -= Button_PointerUp;
		button.InteractibilityChanged -= Button_InteractabilityChanged; 
	}

	//-----------------------------------------------------------------------------------------
	// Event Handlers:
	//-----------------------------------------------------------------------------------------

	private void Button_PointerEntered() {
		isHovered = true;

		if (!button.IsInteractable()) return; 

		SetStyle(hoveredStyle);
	}

	private void Button_PointerExited() {
		isHovered = false;

		if (!button.IsInteractable()) return; 

		SetStyle(normalStyle);
	}

	private void Button_PointerDown() {

		if (!button.IsInteractable()) return; 

		SetStyle(pressedStyle);
	}

	private void Button_PointerUp() {

		if (!button.IsInteractable()) return; 

		SetStyle(isHovered ? hoveredStyle : normalStyle);
	}

	public void Button_InteractabilityChanged(bool isInteractable) {

		if (isInteractable) {
			SetStyle(isHovered ? hoveredStyle : normalStyle);
		}
		else {
			SetStyle(disabledStyle); 
		}
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