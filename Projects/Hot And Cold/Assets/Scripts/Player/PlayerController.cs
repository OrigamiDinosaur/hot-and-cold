using UnityEngine;
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
		Gameplay,
		Ending
	}

	//-----------------------------------------------------------------------------------------
	// Inspector Variables:
	//-----------------------------------------------------------------------------------------

	[Header("References")]

	[SerializeField] protected CharacterController cc;
	[SerializeField] protected Animator animator; 
	[SerializeField] protected Transform robotRoot;

	[Header("Movement")]

	[SerializeField] protected float movementSpeed;

	[Header("Search")]

	[SerializeField] protected string defaultSearchDescription;
	[SerializeField] protected RangeValue[] searchRanges;

	[Header("Audio")]

	[SerializeField] protected AudioClip pingSfx;
	[SerializeField] protected AudioClip treasureFoundSfx;

	//-----------------------------------------------------------------------------------------
	// Private Fields:
	//-----------------------------------------------------------------------------------------

	private States state = States.PreInit; 

	private Treasure currentTreasure;

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
	// Public Methods:
	//-----------------------------------------------------------------------------------------

	public void SetTreasure(Treasure inTreasure) {
		currentTreasure = inTreasure;
	}

	public void StartGame() {
		ChangeStates(States.Gameplay);
	}

	public void StopGame() {
		ChangeStates(States.Ending);
	}

	public void ResetGame() {

		transform.position = startingPosition;
	}

	public void UpdateMovement(Vector3 movementDirection) {
		if (state != States.Gameplay) return; 

		if (movementDirection == Vector3.zero) {
			animator.SetFloat(SPEED_PARAMETER, 0.0f);
			return; 
		}

		robotRoot.transform.forward = movementDirection;
		animator.SetFloat(SPEED_PARAMETER, 1.0f);

		Vector3 movement = (movementDirection * movementSpeed) * Time.deltaTime;

		cc.Move(movement);
	}

	public void Search() {
		if (state != States.Gameplay) return; 

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
			GameGuiController.TreasureFoundView.ShowTreasureFoundDialogue(currentTreasure.TreasureAsset);

			AudioSource.PlayClipAtPoint(treasureFoundSfx, transform.position);

			CollectResources();
			currentTreasure.Collected();
		}
		else {

			AudioSource.PlayClipAtPoint(pingSfx, transform.position);

			GameGuiController.TreasureWarmthView.ShowTreasureWarmthDialogue(searchDescription);
		}
	}

	//-----------------------------------------------------------------------------------------
	// State Methods:
	//-----------------------------------------------------------------------------------------

	private void ChangeStates(States newState) {
		if (newState == state) return;

		state = newState;

		switch (state) {
			case States.Gameplay:
				StateGameplay_Enter();
				break;
			case States.Ending:
				StateEnding_Enter();
				break;
		}
	}

	private void UpdateStates() {

		switch (state) {
			case States.Gameplay:
				StateGameplay_Update();
				break;
		}
	}

	private void StateGameplay_Enter() {

		totalGoldFound = 0;
		totalScrapFound = 0;
	}

	private void StateGameplay_Update() {

		cc.Move(Vector3.down * 9.98f * Time.deltaTime);
	}

	private void StateEnding_Enter() {
		animator.SetFloat(SPEED_PARAMETER, 0.0f);

		GameState.AddGold(totalGoldFound);
		GameState.AddScrap(totalScrapFound);

		SaveDataHandler.Save();
	}

	//-----------------------------------------------------------------------------------------
	// Private Methods:
	//-----------------------------------------------------------------------------------------

	private void CollectResources() {

		ItemValue itemValue = currentTreasure.TreasureAsset.ItemValue;

		switch (itemValue.currency) {
			case Currencies.Gold:
				totalGoldFound += itemValue.value;
				GameGuiController.ScoreView.SetGoldValue(totalGoldFound);
				break;
			case Currencies.Scrap:
				totalScrapFound += itemValue.value; 
				GameGuiController.ScoreView.SetScrapValue(totalScrapFound);
				break;
		}
	}
}