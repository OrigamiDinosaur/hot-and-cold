using Apache.Core;
using UnityEngine;

public class TreasureSpawner : ApacheComponent {

	//-----------------------------------------------------------------------------------------
	// Inspector Variables:
	//-----------------------------------------------------------------------------------------

	[Header("References")]

	[SerializeField] protected TreasureAsset[] treasureSpawns;
	[SerializeField] protected Treasure treasureHolder;
	[SerializeField] protected BoxCollider spawnBounds;

	[Header("Spawn Options")]

	[SerializeField] protected float[] weightPerRarity;

	//-----------------------------------------------------------------------------------------
	// Public Properties:
	//-----------------------------------------------------------------------------------------

	public Treasure Treasure => treasureHolder;

	//-----------------------------------------------------------------------------------------
	// Private Fields:
	//-----------------------------------------------------------------------------------------

	private float[] spawnThresholds;
	private float maxThreshold;

	//-----------------------------------------------------------------------------------------
	// Unity Lifecycle:
	//-----------------------------------------------------------------------------------------

	protected void Start() {
		
		spawnThresholds = new float[treasureSpawns.Length];

		float currentMaxThreshold = 0; 

		for (int i = 0; i < treasureSpawns.Length; i++) {

			int rarityIndex = (int)treasureSpawns[i].ItemRarity;

			spawnThresholds[i] = currentMaxThreshold + weightPerRarity[rarityIndex];
			currentMaxThreshold = spawnThresholds[i]; 
		}

		maxThreshold = currentMaxThreshold; 
	}
	
	//-----------------------------------------------------------------------------------------
	// Public Methods:
	//-----------------------------------------------------------------------------------------

	public void SpawnTreasure() {

		// generate a random location within our bounds.
		Vector3 spawnLocation = spawnBounds.transform.position + spawnBounds.center;
		float randomX = Random.Range(-(spawnBounds.size.x / 2.0f), (spawnBounds.size.x / 2.0f));
		float randomZ = Random.Range(-(spawnBounds.size.z / 2.0f), (spawnBounds.size.z / 2.0f));

		spawnLocation.x += randomX;
		spawnLocation.z += randomZ;

		// apply our location to our treasure.
		treasureHolder.transform.position = spawnLocation;

		// generate our treasure. 
		float randomTreasureWeight = Random.Range(0.0f, maxThreshold);

		for (int i = 0; i < spawnThresholds.Length; i++) {
			if (randomTreasureWeight < spawnThresholds[i]) {

				treasureHolder.SetTreasureAsset(treasureSpawns[i]);
				break; 
			}
		}
	}

	//-----------------------------------------------------------------------------------------
	// Editor Methods:
	//-----------------------------------------------------------------------------------------

	[ApacheButton]
	public void ForceSpawn() {
		SpawnTreasure();
	}
} 