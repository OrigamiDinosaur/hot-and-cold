using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class AppController {

	//-----------------------------------------------------------------------------------------
	// Public Properties:
	//-----------------------------------------------------------------------------------------

	public static Scenes Scene { get; private set; } = Scenes.Unknown;

	//-----------------------------------------------------------------------------------------
	// Public Methods:
	//-----------------------------------------------------------------------------------------

	static AppController() {
	}

	//-----------------------------------------------------------------------------------------
	// Public Methods:
	//-----------------------------------------------------------------------------------------

	public static void LoadScene(Scenes sceneToLoad, Action<Scenes> onSceneExit = null, bool shouldTransition = true) {

		// encapsule load functionality into action to save on code duplication.
		// ReSharper disable once ConvertToLocalFunction
		Action invokeOnSceneExitAndLoadScene = () => {
			onSceneExit?.Invoke(sceneToLoad);
			SceneManager.LoadScene((int)sceneToLoad);
		};

		// if we're not transitioning, just immediately invoke delegate and load scene and return.
		if (!shouldTransition) {
			invokeOnSceneExitAndLoadScene();
			return;
		}

		// we're fading, so fade out and then invoke delegate and load scene.
		SceneTransitionController.TransitionOut(invokeOnSceneExitAndLoadScene);
	}

	public static void SetNewlyActiveScene(Scenes activeScene) {
		Scene = activeScene;
	}
}