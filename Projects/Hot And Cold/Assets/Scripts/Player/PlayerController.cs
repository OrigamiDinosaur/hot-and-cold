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
		
		totalGoldFound = 0;
		totalScrapFound = 0;

		ChangeStates(States.Searching);
	}

	public void StopGame() {

		if (state == States.Drilling) GameGuiController.DrillingView.DismissView();

		ChangeStates(States.Ending);
	}

	public void ResetGame() {

		transform.position = startingPosition;
	}

	public void UpdateMovement(Vector3 movementDirection) {
		if (state != States.Searching) return; 

		if (movementDirection == Vector3.zero) {
			animator.SetFloat(SPEED_PARAMETER, 0.0f);
			return; 
		}

		robotRoot.transform.forward = movementDirection;
		animator.SetFloat(SPEED_PARAMETER, 1.0f);

		Vector3 movement = (movementDirection * currentAttributes.MoveSpeed) * Time.deltaTime;

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

		cc.Move(Vector3.down * 9.98f * Time.deltaTime);
	}

	private void StateDrilling_Enter() {

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

		animator.SetFloat(SPEED_PARAMETER, 0.0f);

		GameState.AddGold(totalGoldFound);
		GameState.AddScrap(totalScrapFound);

		GameState.SaveData();
	}

	//-----------------------------------------------------------------------------------------
	// Private Methods:
	//-----------------------------------------------------------------------------------------

	private void Search() {

		Vector3 treasurePosition = currentTreasure.transform.position;
		treasurePosition.y = transform.position.y;

		float distanceFromTreasure = Vector3.Distance(transform.position, treasurePosition);
		
		string searchDescription = defaultSearchDescription;

		bool didFindTreasure = false; 

		for (int i = 0; i < searchRanges.Length; i++) {

			if (distanceFromTreasure < searchRanges[i].range) {

				searchDescription = searchRanges[i].description;

				if (i == 0) didFindTreasure = true;
				break;
			}
		}
		
		if (didFindTreasure) {
			

			ChangeStates(States.Drilling);
		}
		else {

			pingSfx.Play();

			GameGuiController.TreasureWarmthView.ShowTreasureWarmthDialogue(searchDescription);
		}
	}

	private void Drill() {
		
		if (Time.time < nextDrillTime) return;
		
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

		ItemValue itemValue = currentTreasure.TreasureAsset.ItemValue;

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