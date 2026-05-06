using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class HatHandler : MonoBehaviour {

	//-----------------------------------------------------------------------------------------
	// Inspector Variables:
	//-----------------------------------------------------------------------------------------

	[SerializeField] protected Transform hatRoot; 

	//-----------------------------------------------------------------------------------------
	// Private Fields:
	//-----------------------------------------------------------------------------------------

	private AsyncOperationHandle<GameObject> hatLoadOpHandle;
	private GameObject hatInstance; 
	
	//-----------------------------------------------------------------------------------------
	// Public Methods:
	//-----------------------------------------------------------------------------------------

	protected void OnDisable() {
		if (hatLoadOpHandle.IsValid()) hatLoadOpHandle.Completed -= HatLoadOpHandle_Completed; 
	}

	//-----------------------------------------------------------------------------------------
	// Public Methods:
	//-----------------------------------------------------------------------------------------

	public void LoadHat(AssetReferenceGameObject assetReference) {

		// if we have a hat instance, destroy it and release its operation handle. 
		if (hatInstance != null) {
			Destroy(hatInstance);
			Addressables.ReleaseInstance(hatLoadOpHandle); 
		}

		// start loading our hat asset. 
		hatLoadOpHandle = Addressables.LoadAssetAsync<GameObject>(assetReference);
		hatLoadOpHandle.Completed += HatLoadOpHandle_Completed; 
	}

	//-----------------------------------------------------------------------------------------
	// Event Handlers:
	//-----------------------------------------------------------------------------------------

	private void HatLoadOpHandle_Completed(AsyncOperationHandle<GameObject> asyncOperationHandle) {

		// if our loading is complete, instantiate a hat!
		if (asyncOperationHandle.Status == AsyncOperationStatus.Succeeded) {
			hatInstance = Instantiate(asyncOperationHandle.Result, hatRoot); 
		}
	}
}