using System.Collections.Generic;
using Apache.Core;
using UnityEngine;

public class SceneControllerBase : ApacheComponent {

	//-----------------------------------------------------------------------------------------
	// Inspector Variables:
	//-----------------------------------------------------------------------------------------

	[Header("Base")]

	[SerializeField] protected List<Subscenes> requiredSubscenes;
	
	//-----------------------------------------------------------------------------------------
	// Protected Fields:
	//-----------------------------------------------------------------------------------------

	protected bool isLoadingScene;
	protected bool isQuitting;

	protected bool hasLoadedSubscenes;

	//-----------------------------------------------------------------------------------------
	// Private Fields:
	//-----------------------------------------------------------------------------------------

	private bool hasLostFocus;

	//-----------------------------------------------------------------------------------------
	// Public Properties:
	//-----------------------------------------------------------------------------------------

	public List<Subscenes> RequiredSubscenes => requiredSubscenes;

	//-----------------------------------------------------------------------------------------
	// Protected Properties:
	//-----------------------------------------------------------------------------------------

	protected virtual bool CanLoadScene => true;
	protected virtual bool CanQuit => true;

	//-----------------------------------------------------------------------------------------
	// Unity Lifecycle:
	//-----------------------------------------------------------------------------------------

	protected virtual void Awake() {

		// load required subscenes, even if we don't have any to load.
		// N.B. by always doing this even if we don't have subscenes, we can always rely on the Subscenes_Loaded event handler as a useful hook.
		SubscenesController.LoadSubscenes(requiredSubscenes, this, Subscenes_Loaded);
	}
	
	protected virtual void Update() {
		
	}

	protected virtual void OnApplicationFocus(bool hasGainedFocusElseLost) {

		// if we gain focus after losing it, reload settings.
		if (hasLostFocus && hasGainedFocusElseLost) {
			Settings.ReloadAllSettings();

			// N.B. we don't need to config here because calling reload invokes the loaded event.
		}

		// flag whether we've lost.
		hasLostFocus = !hasGainedFocusElseLost;
	}

	//-----------------------------------------------------------------------------------------
	// Event Handlers:
	//-----------------------------------------------------------------------------------------
	
	private void Subscenes_Loaded() {
		hasLoadedSubscenes = true;
		HandleSubscenesLoaded();
		SubscenesController.OnSubscenesInited();
	}

	//-----------------------------------------------------------------------------------------
	// Protected Methods:
	//-----------------------------------------------------------------------------------------

	/// <summary>Override (calling base) to configure the scene with updated settings values.</summary>
	protected virtual void ConfigFromSettings() {

		// show/hide cursor based on settings and whether we're in the editor.
		Cursor.visible = Settings.Unity.ShowCursor || Application.isEditor;

		// set VSync settings.
		QualitySettings.vSyncCount = (Settings.Unity.EnableVSync) ? 1 : 0;
	}

	/// <summary>
	/// Override to handle post-subscene-load logic. This may include accessing and configuring subscene controllers and components.
	/// </summary>
	protected virtual void HandleSubscenesLoaded() { }

	/// <summary>Loads a given scene with (default) or without a scene transition.</summary>
	/// <param name="scene">The scene to load.</param>
	/// <param name="shouldTransition">Whether to transition between scenes. Default is true.</param>
	protected virtual void LoadScene(Scenes scene, bool shouldTransition = true) {
		if (!CanLoadScene || isLoadingScene) return;
		isLoadingScene = true;
		AppController.LoadScene(scene, HandleSceneExit, shouldTransition);
	}

	/// <summary>Reloads the current scene.</summary>
	/// <param name="shouldTransition">Whether to transition between scenes. Default is true.</param>
	protected virtual void ReloadScene(bool shouldTransition = true) {
		LoadScene(AppController.Scene, shouldTransition);
	}

	/// <summary>Override to handle scene cleanup and similar logic just before the given scene is loaded.</summary>
	/// <param name="nextScene">The scene which will be loaded after this method is called.</param>
	protected virtual void HandleSceneExit(Scenes nextScene) { }
}