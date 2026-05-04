using UnityEngine;

[System.Serializable]
public class ItemValue {

	public Currencies currency;
	public int value; 
}

[CreateAssetMenu(fileName = SCRIPTABLE_OBJECT_NAME, menuName = MENU_NAME_PREFIX + SCRIPTABLE_OBJECT_NAME, order = MENU_STARTING_ORDER + 12)]
public class TreasureAsset : ScriptableObjectBase {

	//-----------------------------------------------------------------------------------------
	// Constants:
	//-----------------------------------------------------------------------------------------

	private const string SCRIPTABLE_OBJECT_NAME = "Treasure"; 

	//-----------------------------------------------------------------------------------------
	// Inspector Variables:
	//-----------------------------------------------------------------------------------------

	[Header("Details")]

	[SerializeField] protected string itemName;
	[SerializeField] protected Rarities itemRarity;
	[SerializeField] protected ItemValue itemValue;
	[SerializeField] protected int depthValue;

	//-----------------------------------------------------------------------------------------
	// Public Properties:
	//-----------------------------------------------------------------------------------------

	public string ItemName => itemName;
	public Rarities ItemRarity => itemRarity;
	public ItemValue ItemValue => itemValue;
	public int DepthValue => depthValue;
}