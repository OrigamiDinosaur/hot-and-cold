using System.Collections.Generic;

public class GameState : Singleton<GameState> {

	//-----------------------------------------------------------------------------------------
	// Private Fields:
	//-----------------------------------------------------------------------------------------

	private GameData gameData;

	//-----------------------------------------------------------------------------------------
	// Public Properties:
	//-----------------------------------------------------------------------------------------

	public static GameData GameData => Instance.gameData;

	//-----------------------------------------------------------------------------------------
	// Public Methods:
	//-----------------------------------------------------------------------------------------

	public static void AddGold(int goldToAdd) {
		GameData.PlayerGold += goldToAdd;
	}

	public static void RemoveGold(int goldToRemove) {
		GameData.PlayerGold -= goldToRemove; 
	}

	public static void AddScrap(int scrapToAdd) {
		GameData.PlayerScrap += scrapToAdd;
	}

	public static void RemoveScrap(int scrapToRemove) {
		GameData.PlayerScrap -= scrapToRemove;
	}

	public static void SetDrillLevel(int level) {
		GameData.DrillLevel = level; 
	}

	public static void SetEngineLevel(int level) {
		GameData.EngineLevel = level; 
	}

	public static void AddUnlockedHat(int hatId) {

		// create an empty array.
		List<int> hatIds = new List<int>();

		// fill it with our current array of hats.
		if (GameData.UnlockedHatIds.Length > 0) {
			hatIds.AddRange(GameData.UnlockedHatIds);
		}

		// add our new id.
		hatIds.Add(hatId);

		// convert our list back to an array. 
		GameData.UnlockedHatIds = hatIds.ToArray(); 
	}

	public static void SetUnlockedHat(int hatId) {
		GameData.CurrentlyEquippedHat = hatId; 
	}

	public static void Init() {
		
		// init our game data
		Instance.gameData = new GameData(); 

		// attempt to load our game data. 
		if (!SaveDataHandler.Load(out Instance.gameData)) {
			
			// if we don't manage to find our game data, reset values.
			GameData.ResetData();
		}
	}

	public static void SaveData() {
		SaveDataHandler.Save(GameData); 
	}
}