using System;

[System.Serializable]
public class GameData {

	//-----------------------------------------------------------------------------------------
	// Public Fields:
	//-----------------------------------------------------------------------------------------

	public int PlayerGold;
	public int PlayerScrap;

	public int DrillLevel;
	public int EngineLevel;

	public int[] UnlockedHatIds;

	public int CurrentlyEquippedHat;

	//-----------------------------------------------------------------------------------------
	// Public Methods:
	//-----------------------------------------------------------------------------------------

	public void ResetData() {

		PlayerGold = 1000;
		PlayerScrap = 1000;

		DrillLevel = 0;
		EngineLevel = 0;

		UnlockedHatIds = Array.Empty<int>(); 
		CurrentlyEquippedHat = -1; 
	}
}