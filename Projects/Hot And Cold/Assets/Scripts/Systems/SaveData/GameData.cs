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

		PlayerGold = 0;
		PlayerScrap = 0;

		DrillLevel = 1;
		EngineLevel = 1;

		CurrentlyEquippedHat = -1; 
	}
}