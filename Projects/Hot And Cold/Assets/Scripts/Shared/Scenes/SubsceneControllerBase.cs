using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Apache.Core {

	/// <summary>
	/// An optional top-level controller for a subscene. This can be used to configure a scene for when it functions as a subscene, or leave it untouched
	/// when it is not acting as a subscene. In addition, derived classes of <c>SubsceneControllerBase</c> work well as a top-level hook which super scenes
	/// access when this scene is loaded as a subscene. From there, other controllers in this subscene, or individual components, can be accessed and
	/// interacted with.
	/// </summary>
	public class SubsceneControllerBase : ApacheComponent {

		//-----------------------------------------------------------------------------------------
		// Constants:
		//-----------------------------------------------------------------------------------------

		private const string UNTAGGED_TAG = "Untagged";

		//-----------------------------------------------------------------------------------------
		// Inspector Variables:
		//-----------------------------------------------------------------------------------------

		[Header("To Disable")]

		[SerializeField] protected GameObject[] gameObjectsToDisable;
		[SerializeField] protected MonoBehaviour[] componentsToDisable;

		[Header("To Destroy")]

		[SerializeField] protected Object[] objectsToDestroy;
		
		[Header("Main Camera")]

		[SerializeField] protected Camera mainCamera;
		[SerializeField] protected bool makeMainCameraNotMain;

		[ApacheSpace]

		[SerializeField] protected AudioListener audioListener;
		[SerializeField] protected bool disableAudioListener;

		[Header("Options")]
		
		[Tooltip("Given that initialising as a subscene can result in game objects being disabled and/or destroyed, it may be desirable to " +
		         "not perform this work when loading the subscene in the editor. Therefore this value defaults to false.")]
		[SerializeField] protected bool initAsSubsceneInEditor;

		//-----------------------------------------------------------------------------------------
		// Protected Fields:
		//-----------------------------------------------------------------------------------------

		/// <summary>Is the scene which this controller represents currently acting as a subscene?</summary>
		protected bool isSubscene;

		//-----------------------------------------------------------------------------------------
		// Unity Lifecycle:
		//-----------------------------------------------------------------------------------------

		protected virtual void Awake() {
			
			// if we awake and don't have a scene value set, back out, as we're definitely not functioning as a subscene.
			if (AppController.Scene == Scenes.Unknown) return;

			// init as a subscene right away.
			InitAsSubscene();
		}

		//-----------------------------------------------------------------------------------------
		// Public Methods:
		//-----------------------------------------------------------------------------------------

		public virtual void InitAsSubscene() {
			
			// if we've already determined we're a subscene, we've inited, so back out.
			if (isSubscene) return;

			// if we're not playing, so in the editor, and we don't init in the editor, don't perform the below object set up.
			if (!Application.isPlaying && !initAsSubsceneInEditor) {
				isSubscene = true;
				return;
			}

			// in the editor, make an undo step.
	#if UNITY_EDITOR
			string undoGroupName = "Init " + gameObject.scene.name + " as Subscene";
			Undo.SetCurrentGroupName(undoGroupName);
			int undoGroupId = Undo.GetCurrentGroup();
	#endif

			// disable game objects and components we don't want when we act as a subscene.
			foreach (GameObject gameObjectToDisable in gameObjectsToDisable) {
	#if UNITY_EDITOR
				Undo.RecordObject(gameObjectToDisable, undoGroupName);
	#endif
				gameObjectToDisable.SetActive(false);
			}
			foreach (MonoBehaviour subsceneComponentToDisable in componentsToDisable) {
	#if UNITY_EDITOR
				Undo.RecordObject(subsceneComponentToDisable, undoGroupName);
	#endif
				subsceneComponentToDisable.enabled = false;
			}

			// destroy game objects and components we don't want when we act as a subscene.
			foreach (Object objectToDestroy in objectsToDestroy) {
				if (Application.isEditor) {
	#if UNITY_EDITOR
					Undo.DestroyObjectImmediate(objectToDestroy);
	#endif
				}
				else {
					Destroy(objectToDestroy);
				}
			}

			// handle making main camera no longer main but untagged instead.
			if (mainCamera != null && makeMainCameraNotMain) {
	#if UNITY_EDITOR
				Undo.RecordObject(mainCamera, undoGroupName);
	#endif
				mainCamera.tag = UNTAGGED_TAG;
			}

			// handle disabling of the audio listener.
			if (audioListener != null && disableAudioListener) {
	#if UNITY_EDITOR
				Undo.RecordObject(audioListener, undoGroupName);
	#endif
				audioListener.enabled = false;
			}

			// in the editor, collapse all undo operations into a single one.
	#if UNITY_EDITOR
			Undo.CollapseUndoOperations(undoGroupId);
	#endif

			// flag we've now initialised as a subscene.
			isSubscene = true;
		}
	}
}