
using UnityEngine;

public class GameState : Singleton<GameState> {

	//-----------------------------------------------------------------------------------------
	// Private Fields:
	//-----------------------------------------------------------------------------------------

	private int playerGold;
	private int playerScrap;

	//-----------------------------------------------------------------------------------------
	// Public Properties:
	//-----------------------------------------------------------------------------------------

	public static int PlayerGold => Instance.playerGold;
	public static int PlayerScrap => Instance.playerScrap;

	//-----------------------------------------------------------------------------------------
	// Public Methods:
	//-----------------------------------------------------------------------------------------

	public static void AddGold(int goldToAdd) {
		Instance.playerGold += goldToAdd;

		SaveDataHandler.SaveData.PlayerGold = Instance.playerGold;
	}

	public static void ResetGold() {
		Instance.playerGold = 0;
		
		SaveDataHandler.SaveData.PlayerGold = 0;
	}

	public static void AddScrap(int scrapToAdd) {
		Instance.playerScrap += scrapToAdd; 

		SaveDataHandler.SaveData.PlayerScrap = Instance.playerScrap;
	}

	public static void ResetScrap() {
		Instance.playerScrap = 0;

		SaveDataHandler.SaveData.PlayerScrap = Instance.playerScrap;
	}

	public static void Init() {

		// reset our values;
		ResetGold();
		ResetScrap();

		// attempt to load our game data. 
		if (SaveDataHandler.Load()) {

			SaveData saveData = SaveDataHandler.SaveData;

			AddGold(saveData.PlayerGold);
			AddScrap(saveData.PlayerScrap); 
		}
	}
}