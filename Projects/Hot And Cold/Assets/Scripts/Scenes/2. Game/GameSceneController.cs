using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneController : MonoBehaviour {

	//-----------------------------------------------------------------------------------------
	// Type Definitions:
	//-----------------------------------------------------------------------------------------

	private enum States {
		PreInit,
		Menu,
		MenuClose,
		TransitionToShop,
		Shop,
		TransitionToGame,
		Game,
		GameEnd,
		TransitionToOutroScreen,
		OutroScreen,
		TransitionToReset,
		Reset,
		TransitionToMenu,
		Exit
	}

	//-----------------------------------------------------------------------------------------
	// Inspector Variables:
	//-----------------------------------------------------------------------------------------

	[Header("References")]

	[SerializeField] protected TreasureSpawner treasureSpawner;
	[SerializeField] protected PlayerController playerController;

	[SerializeField] protected CinemachineCamera introCamera;
	[SerializeField] protected CinemachineCamera gameplayCamera;

	[Header("Transition To Game")]

	[SerializeField] protected float transitionToGameDuration;

	[Header("Game")]

	[SerializeField] protected float gameDuration;

	[Header("Game End")]

	[SerializeField] protected float gameEndDuration;

	[Header("Reset")]

	[SerializeField] protected float resetDuration;

	//-----------------------------------------------------------------------------------------
	// Private Fields:
	//-----------------------------------------------------------------------------------------

	private States state = States.PreInit;

	private GameMenuView gameMenuView;
	private OutroScreenView outroScreenView;
	private ShopMenuView shopMenuView;

	private float gameStartTime;
	private float gameEndTime;

	private GameSequence sequence;

	//-----------------------------------------------------------------------------------------
	// Unity Lifecycle:
	//-----------------------------------------------------------------------------------------

	protected void OnEnable() {

		gameMenuView = GameGuiController.GameMenuView;

		gameMenuView.PlayButtonClicked += MenuView_PlayButtonClicked;
		gameMenuView.ShopButtonClicked += MenuView_ShopButtonClicked;
		gameMenuView.ExitButtonClicked += MenuView_ExitButtonClicked;
		gameMenuView.TransitionCompleted += MenuView_TransitionCompleted;

		outroScreenView = GameGuiController.OutroScreenView;

		outroScreenView.TransitionCompleted += OutroScreenView_TransitionCompleted;
		outroScreenView.ValuesPresented += OutroScreenView_ValuesPresented;

		shopMenuView = GameGuiController.ShopMenuView;
	}

	protected void Start() {

		sequence = new GameSequence(this);
		
		InitGameState();

		ChangeStates(States.Menu);
	}

	protected void Update() {

		UpdateStates();
	}

	protected void OnDisable() {

		gameMenuView.PlayButtonClicked -= MenuView_PlayButtonClicked;
		gameMenuView.ShopButtonClicked -= MenuView_ShopButtonClicked;
		gameMenuView.ExitButtonClicked -= MenuView_ExitButtonClicked;
		gameMenuView.TransitionCompleted -= MenuView_TransitionCompleted;

		outroScreenView.TransitionCompleted -= OutroScreenView_TransitionCompleted;
		outroScreenView.ValuesPresented -= OutroScreenView_ValuesPresented;
	}

	//-----------------------------------------------------------------------------------------
	// Event Handlers:
	//-----------------------------------------------------------------------------------------

	private void MenuView_PlayButtonClicked() {
		if (state != States.Menu) return;

		ChangeStates(States.MenuClose);
	}

	private void MenuView_ShopButtonClicked() {
		if (state != States.Menu) return; 

		ChangeStates(States.TransitionToShop);
	}

	private void MenuView_ExitButtonClicked() {
		if (state != States.Menu) return;

		ChangeStates(States.Exit);
	}

	private void MenuView_TransitionCompleted() {

		// todo: convert to switch if a third state arrises. 
		if (state == States.MenuClose) {

			ChangeStates(States.TransitionToGame);
		}
		else if (state == States.Exit) {

			SceneManager.LoadScene(0);
		}

		switch (state) {
			case States.MenuClose:
				ChangeStates(States.TransitionToGame);
				break;
			case States.Exit:
				SceneManager.LoadScene(0);
				break;
			case States.TransitionToMenu:
				ChangeStates(States.Menu); 
				break;
		}
	}

	private void OutroScreenView_TransitionCompleted() {
		
		if (state == States.TransitionToOutroScreen) {

			ChangeStates(States.OutroScreen);
		}
		else if (state == States.TransitionToReset) {

			ChangeStates(States.Reset); 
		}
	}

	private void OutroScreenView_ValuesPresented() {
		if (state != States.OutroScreen) return;

		ChangeStates(States.TransitionToReset);
	}

	//-----------------------------------------------------------------------------------------
	// State Methods:
	//-----------------------------------------------------------------------------------------

	private void ChangeStates(States newState) {
		if (state == newState) return;

		state = newState;

		switch (state) {
			case States.Menu:
				break;
			case States.MenuClose:
				StateMenuClose_Enter();
				break;
			case States.TransitionToShop:
				StateTransitionToShop_Enter();
				break;
			case States.Shop:
				StateShop_Enter();
				break;
			case States.TransitionToGame:
				StateTransitionToGame_Enter();
				break;
			case States.Game:
				StateGame_Enter();
				break;
			case States.GameEnd:
				StateGameEnd_Enter();
				break; 
			case States.TransitionToOutroScreen:
				StateTransitionToOutroScreen_Enter();
				break;
			case States.OutroScreen:
				StateOutroScreen_Enter();
				break;
			case States.TransitionToReset:
				StateTransitionToReset_Enter();
				break;
			case States.Reset:
				StateReset_Enter();
				break;
			case States.TransitionToMenu:
				StateTransitionToMenu_Enter();
				break;
			case States.Exit:
				StateExit_Enter();
				break;
		}
	}

	private void UpdateStates() {

		switch (state) {
			
			case States.Game:
				StateGame_Update();
				break;
		}
	}

	private void StateMenuClose_Enter() {

		gameMenuView.SlideOffLeft();
	}

	private void StateTransitionToShop_Enter() {

		gameMenuView.SlideOffLeft();

		shopMenuView.SetCurrencyValues(playerController.TotalGoldFound, playerController.TotalScrapFound);

		shopMenuView.ShowHideView(true);
		shopMenuView.SlideOnRight();
	}

	private void StateShop_Enter() {

		shopMenuView.SetButtonsEnabled(true); 
	}

	private void StateTransitionToGame_Enter() {
		
		introCamera.Priority = 0;
		introCamera.enabled = false;
		
		treasureSpawner.SpawnTreasure();

		GameGuiController.TimeView.SetRemainingTime(gameDuration);
		GameGuiController.TimeView.PresentView();

		GameGuiController.ScoreView.ResetValues();
		GameGuiController.ScoreView.PresentView();

		sequence.Do(transitionToGameDuration, () => {
			ChangeStates(States.Game);
		});
	}

	private void StateGame_Enter() {
		
		playerController.SetTreasure(treasureSpawner.Treasure);
		playerController.StartGame();

		gameStartTime = Time.time;
		gameEndTime = gameStartTime + gameDuration; 
	}

	private void StateGame_Update() {

		float remainingTime = gameEndTime - Time.time;

		GameGuiController.TimeView.SetRemainingTime(remainingTime);

		if (Time.time >= gameEndTime) {

			ChangeStates(States.GameEnd);
		}
	}

	private void StateGameEnd_Enter() {

		GameGuiController.TimeView.SetRemainingTime(0.0f);
		GameGuiController.TimeView.DismissView();

		GameGuiController.ScoreView.DismissView();

		playerController.StopGame();

		sequence.Do(gameEndDuration, () => {
			ChangeStates(States.TransitionToOutroScreen);
		});
	}

	private void StateTransitionToOutroScreen_Enter() {

		outroScreenView.ShowHideView(true);
		outroScreenView.SlideOnLeft();
	}

	private void StateOutroScreen_Enter() {

		outroScreenView.StartProgress(playerController.TotalGoldFound, playerController.TotalScrapFound);
	}

	private void StateTransitionToReset_Enter() {

		outroScreenView.SlideOffRight();
	}

	private void StateReset_Enter() {

		playerController.ResetGame();

		introCamera.Priority = 2;
		introCamera.enabled = true;

		sequence.Do(resetDuration, () => {
			ChangeStates(States.TransitionToMenu);
		});
	}

	private void StateTransitionToMenu_Enter() {

		gameMenuView.SetButtonsEnabled(true);
		gameMenuView.SlideOnLeft();
	}

	private void StateExit_Enter() {

		gameMenuView.SlideOffLeft();
	}

	//-----------------------------------------------------------------------------------------
	// Private Methods:
	//-----------------------------------------------------------------------------------------

	void InitGameState() {

		// reset our values
		GameState.ResetGold();
		GameState.ResetScrap();

		// todo: add gold and scrap from save. 
	}
}