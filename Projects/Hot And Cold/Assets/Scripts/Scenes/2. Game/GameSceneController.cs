using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
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
	[SerializeField] protected UpgradeHandler upgradeHandler;
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
	
	private float gameStartTime;
	private float gameEndTime;

	private GameSequence sequence;

	private bool hasUnsubbedGameGuiStaticEvents;

	//-----------------------------------------------------------------------------------------
	// Unity Lifecycle:
	//-----------------------------------------------------------------------------------------

	protected void OnEnable() {
		
		GameGuiController.GameMenuView.PlayButtonClicked += MenuView_PlayButtonClicked;
		GameGuiController.GameMenuView.ShopButtonClicked += MenuView_ShopButtonClicked;
		GameGuiController.GameMenuView.ExitButtonClicked += MenuView_ExitButtonClicked;
		GameGuiController.GameMenuView.TransitionCompleted += MenuView_TransitionCompleted;
		
		GameGuiController.OutroScreenView.TransitionCompleted += OutroScreenView_TransitionCompleted;
		GameGuiController.OutroScreenView.ValuesPresented += OutroScreenView_ValuesPresented;
		
		GameGuiController.ShopMenuView.BackButtonClicked += ShopView_BackButtonClicked;

		GameGuiController.SingletonDestroyed += GameGuiController_SingletonDestroyed;
	}

	protected void Start() {

		sequence = new GameSequence(this);
		
		GameState.Init();
		upgradeHandler.Init();

		ChangeStates(States.Menu);
	}

	protected void Update() {

		UpdateStates();
	}

	protected void OnDisable() {
		UnsubGameGuiStaticEvents();
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

	private void ShopView_BackButtonClicked() {
		ChangeStates(States.TransitionToMenu);
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

	private void GameGuiController_SingletonDestroyed() {
		UnsubGameGuiStaticEvents();
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

		// hide our menu. 
		GameGuiController.GameMenuView.SlideOffLeft();
		EventSystem.current.SetSelectedGameObject(null); 
	}

	private void StateTransitionToShop_Enter() {

		// hide our menu. 
		GameGuiController.GameMenuView.SlideOffLeft();

		// update our currency values.
		GameGuiController.ShopMenuView.SetCurrencyValues(GameState.GameData.PlayerGold, GameState.GameData.PlayerScrap);

		// slide our our shop. 
		GameGuiController.ShopMenuView.PresentShop();
	}
	
	private void StateTransitionToGame_Enter() {
		
		// disable our intro camera, forcing our cinemachine camera to transition to our game view. 
		introCamera.Priority = 0;
		introCamera.enabled = false;
		
		// spawn our first treasure. 
		treasureSpawner.SpawnTreasure();

		// set our default time and present our time view. 
		GameGuiController.TimeView.SetRemainingTime(gameDuration);
		GameGuiController.TimeView.PresentView();

		// reset our score values and present our score view. 
		GameGuiController.ScoreView.ResetValues();
		GameGuiController.ScoreView.PresentView();

		// move to our game after delay. 
		sequence.Do(transitionToGameDuration, () => {
			ChangeStates(States.Game);
		});
	}

	private void StateGame_Enter() {
		
		// give the player controller our treasure and start our game. 
		playerController.SetTreasure(treasureSpawner.Treasure);
		playerController.StartGame();

		// set our game start and end time. 
		gameStartTime = Time.time;
		gameEndTime = gameStartTime + gameDuration; 
	}

	private void StateGame_Update() {

		// calculate our remaining time and update our time view. 
		float remainingTime = gameEndTime - Time.time;
		GameGuiController.TimeView.SetRemainingTime(remainingTime);

		// if we're exceeded our game time, end it! 
		if (Time.time >= gameEndTime) {
			ChangeStates(States.GameEnd);
		}
	}

	private void StateGameEnd_Enter() {

		// reset and hide our time view. 
		GameGuiController.TimeView.SetRemainingTime(0.0f);
		GameGuiController.TimeView.DismissView();

		// hide our score view. 
		GameGuiController.ScoreView.DismissView();

		// stop our game. 
		playerController.StopGame();

		// have our upgrade handler update our costs.
		upgradeHandler.GameFinished();

		// move to our transition after a short delay.
		sequence.Do(gameEndDuration, () => {
			ChangeStates(States.TransitionToOutroScreen);
		});
	}

	private void StateTransitionToOutroScreen_Enter() {

		// reset our outro progress, set it visible and slide it on. 
		GameGuiController.OutroScreenView.ResetProgres();
		GameGuiController.OutroScreenView.ShowHideView(true);
		GameGuiController.OutroScreenView.SlideOnLeft();
	}

	private void StateOutroScreen_Enter() {

		// start the outro ticking. 
		GameGuiController.OutroScreenView.StartProgress(playerController.TotalGoldFound, playerController.TotalScrapFound);
	}

	private void StateTransitionToReset_Enter() {

		// hide our outro view. 
		GameGuiController.OutroScreenView.SlideOffRight();
	}

	private void StateReset_Enter() {

		// reset our player position. 
		playerController.ResetGame();

		// reenable our intro camera. 
		introCamera.Priority = 2;
		introCamera.enabled = true;

		// move to transition after a delay. 
		sequence.Do(resetDuration, () => {
			ChangeStates(States.TransitionToMenu);
		});
	}

	private void StateTransitionToMenu_Enter() {
		
		// show our menu. 
		GameGuiController.GameMenuView.SlideOnLeft();
	}

	private void StateExit_Enter() {

		// hide our menu. 
		GameGuiController.GameMenuView.SlideOffLeft();
	}

	//-----------------------------------------------------------------------------------------
	// Private Methods:
	//-----------------------------------------------------------------------------------------

	private void UnsubGameGuiStaticEvents() {
		if (hasUnsubbedGameGuiStaticEvents) return;

		GameGuiController.SingletonDestroyed -= GameGuiController_SingletonDestroyed;

		GameGuiController.GameMenuView.PlayButtonClicked -= MenuView_PlayButtonClicked;
		GameGuiController.GameMenuView.ShopButtonClicked -= MenuView_ShopButtonClicked;
		GameGuiController.GameMenuView.ExitButtonClicked -= MenuView_ExitButtonClicked;
		GameGuiController.GameMenuView.TransitionCompleted -= MenuView_TransitionCompleted;

		GameGuiController.OutroScreenView.TransitionCompleted -= OutroScreenView_TransitionCompleted;
		GameGuiController.OutroScreenView.ValuesPresented -= OutroScreenView_ValuesPresented;

		GameGuiController.ShopMenuView.BackButtonClicked -= ShopView_BackButtonClicked;

		hasUnsubbedGameGuiStaticEvents = true;
	}
}