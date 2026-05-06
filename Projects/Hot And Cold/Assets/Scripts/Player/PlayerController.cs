using UnityEngine;
using UnityEngine.AddressableAssets;
using Vector3 = UnityEngine.Vector3;

[System.Serializable]
public class RangeValue {

	public float range;
	public string description; 
}

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour {

	//-----------------------------------------------------------------------------------------
	// Constants:
	//-----------------------------------------------------------------------------------------

	private static readonly int SPEED_PARAMETER = Animator.StringToHash("Speed");
	private const float GRAVITY_FORCE = 9.98f; 

	//-----------------------------------------------------------------------------------------
	// Type Definitions:
	//-----------------------------------------------------------------------------------------

	private enum States {
		PreInit,
		Waiting,
		Searching,
		Drilling,
		Ending
	}

	//-----------------------------------------------------------------------------------------
	// Inspector Variables:
	//-----------------------------------------------------------------------------------------

	[Header("References")]

	[SerializeField] protected CharacterController cc;
	[SerializeField] protected Animator animator; 
	[SerializeField] protected Transform robotRoot;
	[SerializeField] protected HatHandler hatHandler;
	
	[Header("Search")]

	[SerializeField] protected string defaultSearchDescription;
	[SerializeField] protected RangeValue[] searchRanges;

	[Header("Drilling")]

	[SerializeField] protected float drillRate;

	[Header("Audio")]

	[SerializeField] protected AudioSource pingSfx;
	[SerializeField] protected AudioSource drillSfx;
	[SerializeField] protected AudioSource treasureFoundSfx;

	//-----------------------------------------------------------------------------------------
	// Private Fields:
	//-----------------------------------------------------------------------------------------

	private States state = States.PreInit;

	private PlayerAttributes currentAttributes;

	private Treasure currentTreasure;
	protected int currentDepthValue;

	private float nextDrillTime;

	private Vector3 startingPosition;

	private int totalGoldFound;
	private int totalScrapFound; 

	//-----------------------------------------------------------------------------------------
	// Public Properties:
	//-----------------------------------------------------------------------------------------

	public int TotalGoldFound => totalGoldFound;
	public int TotalScrapFound => totalScrapFound;

	//-----------------------------------------------------------------------------------------
	// Unity Lifecycle:
	//-----------------------------------------------------------------------------------------

	protected void Start() {

		// cache our starting position. 
		startingPosition = transform.position;

		ChangeStates(States.Waiting);
	}

	protected void Update() {
		UpdateStates();
	}

	//-----------------------------------------------------------------------------------------
	// Getters / Setters:
	//-----------------------------------------------------------------------------------------

	public void SetTreasure(Treasure inTreasure) {
		currentTreasure = inTreasure;
	}

	public void SetCurrentAttributes(PlayerAttributes inAttributes) {
		currentAttributes = inAttributes;
	}

	public void SetEquippedHat(AssetReferenceGameObject assetReference) {
		hatHandler.LoadHat(assetReference);
	}

	//-----------------------------------------------------------------------------------------
	// Public Methods:
	//-----------------------------------------------------------------------------------------

	public void StartGame() {
		
		// reset our currencies. 
		totalGoldFound = 0;
		totalScrapFound = 0;

		ChangeStates(States.Searching);
	}

	public void StopGame() {

		// hide our drilling view if need be. 
		if (state == States.Drilling) GameGuiController.DrillingView.DismissView();

		ChangeStates(States.Ending);
	}

	public void ResetGame() {
		transform.position = startingPosition;
	}

	public void UpdateMovement(Vector3 movementDirection) {
		if (state != States.Searching) return; 

		// if we're passed zero value, the player isn't moving so set our animator to still.
		if (movementDirection == Vector3.zero) {
			animator.SetFloat(SPEED_PARAMETER, 0.0f);
			return; 
		}

		// have our robot face the movement direction. 
		robotRoot.transform.forward = movementDirection;

		// set our animator to walk. 
		animator.SetFloat(SPEED_PARAMETER, 1.0f);

		// calculate our movement vector. 
		Vector3 movement = (movementDirection * currentAttributes.MoveSpeed) * Time.deltaTime;

		// move!
		cc.Move(movement);
	}

	public void AttemptSearchOrDill() {
		
		if (state == States.Searching) {
			Search();
		}
		else if (state == States.Drilling) {
			Drill();
		}
	}

	//-----------------------------------------------------------------------------------------
	// State Methods:
	//-----------------------------------------------------------------------------------------

	private void ChangeStates(States newState) {
		if (newState == state) return;

		state = newState;

		switch (state) {
			case States.Drilling:
				StateDrilling_Enter();
				break;
			case States.Ending:
				StateEnding_Enter();
				break;
		}
	}

	private void UpdateStates() {

		switch (state) {
			case States.Searching:
				StateSearching_Update();
				break;
		}
	}

	private void StateSearching_Update() {

		// gravity!
		cc.Move(Vector3.down * GRAVITY_FORCE * Time.deltaTime);
	}

	private void StateDrilling_Enter() {

		// stop our walking. 
		animator.SetFloat(SPEED_PARAMETER, 0.0f);

		// set our depth value and update our gui.
		currentDepthValue = currentTreasure.TreasureAsset.DepthValue;
		GameGuiController.DrillingView.SetDepthValue(currentDepthValue);

		// show the drill view.
		GameGuiController.DrillingView.PresentView();

		// set our first viable drill time. 
		nextDrillTime = Time.time + drillRate;
	}

	private void StateEnding_Enter() {

		// stop us walking. 
		animator.SetFloat(SPEED_PARAMETER, 0.0f);

		// update our game state. 
		GameState.AddGold(totalGoldFound);
		GameState.AddScrap(totalScrapFound);
		GameState.SaveData();
	}

	//-----------------------------------------------------------------------------------------
	// Private Methods:
	//-----------------------------------------------------------------------------------------

	private void Search() {

		// get our treasure position, and adjust its y to match our own. 
		Vector3 treasurePosition = currentTreasure.transform.position;
		treasurePosition.y = transform.position.y;

		// calculate our distance from the treasure. 
		float distanceFromTreasure = Vector3.Distance(transform.position, treasurePosition);
		
		// set our default search description. 
		string searchDescription = defaultSearchDescription;

		// assume we didn't find treasure. 
		bool didFindTreasure = false; 

		// iterate over our search ranges. 
		for (int i = 0; i < searchRanges.Length; i++) {

			// if the distance is less than our range....
			if (distanceFromTreasure < searchRanges[i].range) {

				//... update our searach description. 
				searchDescription = searchRanges[i].description;

				// if its our 0 entry that means we've found treasure. 
				if (i == 0) didFindTreasure = true;

				// stop at first successful check. 
				break;
			}
		}
		
		// if we've found our treasure go to our drilling state. 
		if (didFindTreasure) {
			ChangeStates(States.Drilling);
		}

		// else play our ping sfx, and show our treasure warmth dialogue. 
		else {

			pingSfx.Play();
			GameGuiController.TreasureWarmthView.ShowTreasureWarmthDialogue(searchDescription);
		}
	}

	private void Drill() {
		
		// back out if we've not reached our drill time. 
		if (Time.time < nextDrillTime) return;
		
		// set our next drill time. 
		nextDrillTime = Time.time + drillRate;
		
		// reduce our depth value
		currentDepthValue -= (int)currentAttributes.DrillValue;

		// if we've finished drilling, find our treasure!
		if (currentDepthValue <= 0) {

			GameGuiController.DrillingView.DismissView();
			GameGuiController.TreasureFoundView.ShowTreasureFoundDialogue(currentTreasure.TreasureAsset);

			treasureFoundSfx.Play();

			CollectResources();
			currentTreasure.Collected();

			ChangeStates(States.Searching); 
		}

		// otherwise drill!
		else {

			GameGuiController.DrillingView.SetDepthValue(currentDepthValue);
			drillSfx.Play();
		}
	}

	private void CollectResources() {

		// get our item value. 
		ItemValue itemValue = currentTreasure.TreasureAsset.ItemValue;

		// check which currency we're dealing with, then add the value (adjusted by our bonuses) to our stored currency. 
		switch (itemValue.currency) {
			case Currencies.Gold:
				totalGoldFound += (int)(itemValue.value * (1.0f + currentAttributes.GoldBonus));
				GameGuiController.ScoreView.SetGoldValue(totalGoldFound);
				break;
			case Currencies.Scrap:
				totalScrapFound += (int)(itemValue.value * (1.0f + currentAttributes.ScrapBonus));
				GameGuiController.ScoreView.SetScrapValue(totalScrapFound);
				break;
		}
	}

	
}