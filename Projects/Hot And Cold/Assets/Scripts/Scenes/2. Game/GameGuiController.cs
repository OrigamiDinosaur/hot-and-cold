using UnityEngine;

public class GameGuiController : ComponentSingleton<GameGuiController> {

	//-----------------------------------------------------------------------------------------
	// Inspector Variables:
	//-----------------------------------------------------------------------------------------
	
	[SerializeField] protected GameMenuView gameMenuView;
	[SerializeField] protected ShopMenuView shopMenuView; 
	[SerializeField] protected OutroScreenView outroScreenView;
	[SerializeField] protected TreasureWarmthView treasureWarmthView;
	[SerializeField] protected DrillingView drillingView; 
	[SerializeField] protected TreasureFoundView treasureFoundView;
	[SerializeField] protected TimeView timeView;
	[SerializeField] protected ScoreView scoreView; 

	//-----------------------------------------------------------------------------------------
	// Public Properties:
	//-----------------------------------------------------------------------------------------
	
	public static GameMenuView GameMenuView => Instance.gameMenuView;
	public static ShopMenuView ShopMenuView => Instance.shopMenuView;
	public static OutroScreenView OutroScreenView => Instance.outroScreenView; 
	public static TreasureWarmthView TreasureWarmthView => Instance.treasureWarmthView;
	public static DrillingView DrillingView => Instance.drillingView; 
	public static TreasureFoundView TreasureFoundView => Instance.treasureFoundView;
	public static TimeView TimeView => Instance.timeView;
	public static ScoreView ScoreView => Instance.scoreView; 
}