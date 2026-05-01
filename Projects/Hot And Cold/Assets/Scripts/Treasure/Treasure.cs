using System;
using UnityEngine;

public class Treasure : MonoBehaviour {

	//-----------------------------------------------------------------------------------------
	// Events:
	//-----------------------------------------------------------------------------------------

	public event Action TreasureCollected;

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

	public void Collected() {
		TreasureCollected?.Invoke();
	}
}