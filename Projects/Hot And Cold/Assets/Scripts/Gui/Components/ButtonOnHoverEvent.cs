using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonOnHoverEvent : Button {

	//-----------------------------------------------------------------------------------------
	// Events:
	//-----------------------------------------------------------------------------------------

	public event Action PointerEntered;
	public event Action PointerExited;
	public event Action PointerDown;
	public event Action PointerUp;

	//-----------------------------------------------------------------------------------------
	// Protected Methods:
	//-----------------------------------------------------------------------------------------

	public override void OnPointerEnter(PointerEventData pointerEventData) {
		base.OnPointerEnter(pointerEventData);
		PointerEntered?.Invoke();
	}

	public override void OnPointerExit(PointerEventData pointerEventData) {
		base.OnPointerExit(pointerEventData); 
		PointerExited?.Invoke();
	}

	public override void OnPointerDown(PointerEventData pointerEventData) {
		base.OnPointerDown(pointerEventData);
		PointerDown?.Invoke();
	}

	public override void OnPointerUp(PointerEventData pointerEventData) {
		base.OnPointerUp(pointerEventData);
		PointerUp?.Invoke();
	}
}