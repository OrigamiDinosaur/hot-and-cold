using System.Numerics;
using Apache.Core;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

[System.Serializable]
public class RangeValue {

	public float range;
	public string description; 
}

[RequireComponent(typeof(CharacterController))]
public class PlayerController : ApacheComponent {

	//-----------------------------------------------------------------------------------------
	// Constants:
	//-----------------------------------------------------------------------------------------

	private static readonly int SPEED_PARAMETER = Animator.StringToHash("Speed");

	//-----------------------------------------------------------------------------------------
	// Inspector Variables:
	//-----------------------------------------------------------------------------------------

	[Header("References")]

	[SerializeField] protected CharacterController cc;
	[SerializeField] protected Animator animator; 
	[SerializeField] protected Transform robotRoot;

	[Header("Movement")]

	[SerializeField] protected float movementSpeed;

	[Header("Search")]

	[SerializeField] protected string defaultSearchDescription;
	[SerializeField] protected RangeValue[] searchRanges;

	[Header("Audio")]

	[SerializeField] protected AudioClip pingSfx;
	[SerializeField] protected AudioClip treasureFoundSfx;

	//-----------------------------------------------------------------------------------------
	// Private Fields:
	//-----------------------------------------------------------------------------------------

	private Treasure currentTreasure;

	//-----------------------------------------------------------------------------------------
	// Unity Lifecycle:
	//-----------------------------------------------------------------------------------------

	protected void Update() {

		cc.Move(Vector3.down * 9.98f * Time.deltaTime);
	}

	//-----------------------------------------------------------------------------------------
	// Public Methods:
	//-----------------------------------------------------------------------------------------

	public void SetTreasure(Treasure inTreasure) {
		currentTreasure = inTreasure;
	}

	public void UpdateMovement(Vector3 movementDirection) {

		if (movementDirection == Vector3.zero) {
			animator.SetFloat(SPEED_PARAMETER, 0.0f);
			return; 
		}

		robotRoot.transform.forward = movementDirection;
		animator.SetFloat(SPEED_PARAMETER, 1.0f);

		Vector3 movement = (movementDirection * movementSpeed) * Time.deltaTime;

		cc.Move(movement);
	}

	public void Search() {

		Vector3 treasurePosition = currentTreasure.transform.position;
		treasurePosition.y = transform.position.y;

		float distanceFromTreasure = Vector3.Distance(transform.position, treasurePosition);
		
		string searchDescription = defaultSearchDescription;

		bool didFindTreasure = false; 

		for (int i = 0; i < searchRanges.Length; i++) {

			if (distanceFromTreasure < searchRanges[i].range) {

				searchDescription = searchRanges[i].description;

				if (i == 0) didFindTreasure = true;
				break;
			}
		}
		
		if (didFindTreasure) {
			GameGuiController.TreasureFoundView.ShowTreasureFoundDialogue(currentTreasure.TreasureAsset);

			AudioSource.PlayClipAtPoint(treasureFoundSfx, transform.position);

			currentTreasure.Collected();
		}
		else {

			AudioSource.PlayClipAtPoint(pingSfx, transform.position);

			GameGuiController.TreasureWarmthView.ShowTreasureWarmthDialogue(searchDescription);
		}
	}
}