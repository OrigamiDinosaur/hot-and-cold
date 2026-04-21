using Apache.Core;
using UnityEngine;

public class GameSceneController : ApacheComponent {

	//-----------------------------------------------------------------------------------------
	// Inspector Variables:
	//-----------------------------------------------------------------------------------------

	[SerializeField] protected TreasureSpawner treasureSpawner;
	[SerializeField] protected PlayerController playerController;

	//-----------------------------------------------------------------------------------------
	// Unity Lifecycle:
	//-----------------------------------------------------------------------------------------

	protected void Start() {
		sequence.NextFrame(() => {
			treasureSpawner.SpawnTreasure();
			playerController.SetTreasure(treasureSpawner.Treasure);
		});
	}
}