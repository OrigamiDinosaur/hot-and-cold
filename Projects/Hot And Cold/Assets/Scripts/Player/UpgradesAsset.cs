using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = SCRIPTABLE_OBJECT_NAME, menuName = MENU_NAME_PREFIX + SCRIPTABLE_OBJECT_NAME, order = MENU_STARTING_ORDER + 12)]
public class UpgradesAsset : ScriptableObjectBase {

	//-----------------------------------------------------------------------------------------
	// Constants:
	//-----------------------------------------------------------------------------------------

	private const string SCRIPTABLE_OBJECT_NAME = "Upgrades"; 

	//-----------------------------------------------------------------------------------------
	// Inspector Variables:
	//-----------------------------------------------------------------------------------------

	[Header("Details")]

	[SerializeField] protected string upgradeName;
	
	[SerializeField] protected float[] upgradeValues; 
	
	[SerializeField] protected int[] upgradeGoldCosts;
	[SerializeField] protected int[] upgradeScrapCosts;

	//-----------------------------------------------------------------------------------------
	// Public Properties:
	//-----------------------------------------------------------------------------------------

	public float[] UpgradeValues => upgradeValues;
	public int[] UpgradeGoldCosts => upgradeGoldCosts;
	public int[] UpgradeScrapCosts => upgradeScrapCosts;
}