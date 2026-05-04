using TMPro;
using UnityEngine;

public class HatLine : MonoBehaviour {

	//-----------------------------------------------------------------------------------------
	// Type Definitions:
	//-----------------------------------------------------------------------------------------

	public enum States {
		Unpurchased,
		Unequipped,
		Equipped
	}

	//-----------------------------------------------------------------------------------------
	// Events:
	//-----------------------------------------------------------------------------------------

	public IntAction HatPurchaseRequested;
	public IntAction HatEquipRequested;

	//-----------------------------------------------------------------------------------------
	// Inspector Variables:
	//-----------------------------------------------------------------------------------------

	[Header("References")]

	[SerializeField] protected RectTransform rect;

	[SerializeField] protected TextMeshProUGUI hatNameText;

	[SerializeField] protected OriButton buyButton;
	[SerializeField] protected OriButton equipButton;
	[SerializeField] protected OriButton equippedButton;

	[SerializeField] protected GameObject costTab;

	[SerializeField] protected TextMeshProUGUI goldCostText;
	[SerializeField] protected TextMeshProUGUI scrapCostText;
	[SerializeField] protected TextMeshProUGUI descriptionText;

	//-----------------------------------------------------------------------------------------
	// Private Fields:
	//-----------------------------------------------------------------------------------------

	private int hatId;

	private States state;

	//-----------------------------------------------------------------------------------------
	// Public Properties:
	//-----------------------------------------------------------------------------------------

	public int HatId => hatId;
	public States State => state; 

	//-----------------------------------------------------------------------------------------
	// Unity Lifecycle:
	//-----------------------------------------------------------------------------------------

	protected void OnEnable() {
		buyButton.PointerEntered += Button_PointerEntered;
		buyButton.PointerExited += Button_PointerExited;
		equipButton.PointerEntered += Button_PointerEntered;
		equipButton.PointerExited += Button_PointerExited;
		equippedButton.PointerEntered += Button_PointerEntered;
		equippedButton.PointerExited += Button_PointerExited;
	}

	protected void OnDisable() {
		buyButton.PointerEntered -= Button_PointerEntered;
		buyButton.PointerExited -= Button_PointerExited;
		equipButton.PointerEntered -= Button_PointerEntered;
		equipButton.PointerExited -= Button_PointerExited;
		equippedButton.PointerEntered -= Button_PointerEntered;
		equippedButton.PointerExited -= Button_PointerExited;
	}

	//-----------------------------------------------------------------------------------------
	// Event Handlers - Inpspector:
	//-----------------------------------------------------------------------------------------

	public void BuyButton_Clicked() {
		HatPurchaseRequested?.Invoke(hatId);
	}

	public void EquipButton_Clicked() {
		HatEquipRequested?.Invoke(hatId);
	}

	//-----------------------------------------------------------------------------------------
	// Event Handlers:
	//-----------------------------------------------------------------------------------------

	private void Button_PointerEntered() {
		costTab.SetActive(true); 
	}

	private void Button_PointerExited() {
		costTab.SetActive(false); 
	}

	//-----------------------------------------------------------------------------------------
	// Public Methods:
	//-----------------------------------------------------------------------------------------

	public void SetAnchoredPosition(Vector2 position) {
		rect.anchoredPosition = position;
	}

	public void SetHatId(int inHatId) {
		hatId = inHatId;
	}

	public void SetHatName(string hatName) {
		hatNameText.text = hatName;
	}

	public void SetCost(int goldCost, int scrapCost) {
		goldCostText.text = goldCost.ToString();
		scrapCostText.text = scrapCost.ToString();
	}

	public void SetDescription(string description) {
		descriptionText.text = description;
	}
	
	public void SetState(States newState) {
		if (state == newState) return;

		state = newState;

		buyButton.gameObject.SetActive(state == States.Unpurchased);
		equipButton.gameObject.SetActive(state == States.Unequipped); 
		equippedButton.gameObject.SetActive(state == States.Equipped);
	}

	public void SetCanAffordCost(bool canAffordCost) {
		buyButton.SetInteractable(canAffordCost); 
	}
}