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

		GameGuiController.GameMenuView.SlideOffLeft();
	}

	private void StateTransitionToShop_Enter() {

		GameGuiController.GameMenuView.SlideOffLeft();

		GameGuiController.ShopMenuView.SetCurrencyValues(GameState.GameData.PlayerGold, GameState.GameData.PlayerScrap);

		GameGuiController.ShopMenuView.PresentShop();
	}

	private void StateShop_Enter() {

		//shopMenuView.SetButtonsEnabled(true); 
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

		GameGuiController.OutroScreenView.ShowHideView(true);
		GameGuiController.OutroScreenView.SlideOnLeft();
	}

	private void StateOutroScreen_Enter() {

		GameGuiController.OutroScreenView.StartProgress(playerController.TotalGoldFound, playerController.TotalScrapFound);
	}

	private void StateTransitionToReset_Enter() {

		GameGuiController.OutroScreenView.SlideOffRight();
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

		GameGuiController.GameMenuView.SetButtonsEnabled(true);
		GameGuiController.GameMenuView.SlideOnLeft();
	}

	private void StateExit_Enter() {

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