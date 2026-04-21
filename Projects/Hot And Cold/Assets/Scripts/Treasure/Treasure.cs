using Apache.Core;
using UnityEngine;

public class Treasure : ApacheComponent {

	//-----------------------------------------------------------------------------------------
	// Inspector Variables:
	//-----------------------------------------------------------------------------------------

	[SerializeField] protected string treasureName;

	//-----------------------------------------------------------------------------------------
	// Private Fields:
	//-----------------------------------------------------------------------------------------

	private TreasureAsset treasureAsset; 

	//-----------------------------------------------------------------------------------------
	// Public Properties:
	//-----------------------------------------------------------------------------------------

	public TreasureAsset TreasureAsset => treasureAsset; 

	//-----------------------------------------------------------------------------------------
	// Public Methods:
	//-----------------------------------------------------------------------------------------

	public void SetTreasureAsset(TreasureAsset inAsset) {
		treasureAsset = inAsset;

		treasureName = treasureAsset.ItemName;
	}
}