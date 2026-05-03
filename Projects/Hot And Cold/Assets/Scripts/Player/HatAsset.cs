using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = SCRIPTABLE_OBJECT_NAME, menuName = MENU_NAME_PREFIX + SCRIPTABLE_OBJECT_NAME, order = MENU_STARTING_ORDER + 12)]
public class HatAsset : ScriptableObjectBase {

	//-----------------------------------------------------------------------------------------
	// Constants:
	//-----------------------------------------------------------------------------------------

	private const string SCRIPTABLE_OBJECT_NAME = "Hat";

	//-----------------------------------------------------------------------------------------
	// Inspector Variables:
	//-----------------------------------------------------------------------------------------

	[Header("Details")]

	[SerializeField] protected int hatId;
	[SerializeField] protected AssetReferenceGameObject hatAssetReference;

	[SerializeField] protected string hatName;
	[SerializeField] protected string hatDescription;
	[SerializeField] protected int goldCost;
	[SerializeField] protected int scrapCost;

	[SerializeField] protected PlayerAttributes attributeBonus;

	//-----------------------------------------------------------------------------------------
	// Public Methods:
	//-----------------------------------------------------------------------------------------
	
	public int HatId => hatId;
	public AssetReferenceGameObject HatAssetReference => hatAssetReference;

	public string HatName => hatName;
	public string HatDescription => hatDescription;

	public int GoldCost => goldCost;
	public int ScrapCost => scrapCost;

	public PlayerAttributes AttributeBonus => attributeBonus;
}