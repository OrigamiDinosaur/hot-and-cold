
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
	}

	public static void ResetGold() {
		Instance.playerGold = 0;
	}

	public static void AddScrap(int scrapToAdd) {
		Instance.playerScrap += scrapToAdd; 
	}

	public static void ResetScrap() {
		Instance.playerScrap = 0; 
	}
}