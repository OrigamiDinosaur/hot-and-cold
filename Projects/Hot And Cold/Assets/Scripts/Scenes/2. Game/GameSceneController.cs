using System;
using Apache.Core;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneController : ApacheComponent {

	//-----------------------------------------------------------------------------------------
	// Type Definitions:
	//-----------------------------------------------------------------------------------------

	private enum States {
		PreInit,
		Menu,
		MenuClose,
		TransitionToGame,
		Game,
		GameEnd,
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

	[Header("Transition")]

	[SerializeField] protected float transitionToGameDuration;

	[Header("Game")]

	[SerializeField] protected float gameDuration;

	//-----------------------------------------------------------------------------------------
	// Private Fields:
	//-----------------------------------------------------------------------------------------

	private States state = States.PreInit;

	private GameMenuView gameMenuView;

	private float gameStartTime;
	private float gameEndTime;

	//-----------------------------------------------------------------------------------------
	// Unity Lifecycle:
	//-----------------------------------------------------------------------------------------

	protected void OnEnable() {

		gameMenuView = GameGuiController.GameMenuView;

		gameMenuView.PlayButtonClicked += MenuView_PlayButtonClicked;
		gameMenuView.ShopButtonClicked += MenuView_ShopButtonClicked;
		gameMenuView.ExitButtonClicked += MenuView_ExitButtonClicked;
		gameMenuView.MenuClosed += MenuView_MenuClosed;
	}

	protected void Start() {
		
		ChangeStates(States.Menu);
	}

	protected void Update() {

		UpdateStates();
	}

	protected void OnDisable() {

		gameMenuView.PlayButtonClicked -= MenuView_PlayButtonClicked;
		gameMenuView.ShopButtonClicked -= MenuView_ShopButtonClicked;
		gameMenuView.ExitButtonClicked -= MenuView_ExitButtonClicked;
		gameMenuView.MenuClosed -= MenuView_MenuClosed;
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
	}

	private void MenuView_ExitButtonClicked() {
		if (state != States.Menu) return;

		ChangeStates(States.Exit);
	}

	private void MenuView_MenuClosed() {

		if (state == States.MenuClose) {

			ChangeStates(States.TransitionToGame);
		}
		else if (state == States.Exit) {

			SceneManager.LoadScene(0);
		}
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
			case States.TransitionToGame:
				StateTransitionToGame_Enter();
				break;
			case States.Game:
				StateGame_Enter();
				break;
			case States.GameEnd:
				StateGameEnd_Enter();
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

		gameMenuView.CloseMenu();
	}

	private void StateTransitionToGame_Enter() {
		
		introCamera.Priority = 0;
		introCamera.enabled = false;
		
		treasureSpawner.SpawnTreasure();

		GameGuiController.TimeView.SetRemainingTime(gameDuration);
		GameGuiController.TimeView.PresentView();

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

		playerController.StopGame();
	}

	private void StateExit_Enter() {

		gameMenuView.CloseMenu();
	}
}