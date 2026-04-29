using Apache.Core;
using UnityEngine;

public class GameGuiController : ComponentSingletonProtected<GameGuiController> {

	//-----------------------------------------------------------------------------------------
	// Inspector Variables:
	//-----------------------------------------------------------------------------------------
	
	[SerializeField] protected TreasureWarmthView treasureWarmthView;
	[SerializeField] protected TreasureFoundView treasureFoundView;
	[SerializeField] protected TimeView timeView;
	[SerializeField] protected GameMenuView gameMenuView;

	//-----------------------------------------------------------------------------------------
	// Public Properties:
	//-----------------------------------------------------------------------------------------

	public static TreasureWarmthView TreasureWarmthView => Instance.treasureWarmthView;
	public static TreasureFoundView TreasureFoundView => Instance.treasureFoundView;
	public static TimeView TimeView => Instance.timeView;
	public static GameMenuView GameMenuView => Instance.gameMenuView;
}