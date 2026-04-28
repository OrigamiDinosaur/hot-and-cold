using Apache.Core;
using UnityEngine;

public class GameGuiController : ComponentSingletonProtected<GameGuiController> {

	//-----------------------------------------------------------------------------------------
	// Inspector Variables:
	//-----------------------------------------------------------------------------------------
	
	[SerializeField] protected TreasureWarmthView treasureWarmthView;
	[SerializeField] protected TreasureFoundView treasureFoundView;

	//-----------------------------------------------------------------------------------------
	// Public Properties:
	//-----------------------------------------------------------------------------------------

	public static TreasureWarmthView TreasureWarmthView => Instance.treasureWarmthView;
	public static TreasureFoundView TreasureFoundView => Instance.treasureFoundView;
}