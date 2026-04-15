using System;
using Apache.Core;
using Apache.Core.Extensions;
using UnityEngine;

public class SceneTransitionController : ComponentSingletonProtected<SceneTransitionController> {

	//-----------------------------------------------------------------------------------------
	// Constants:
	//-----------------------------------------------------------------------------------------

	private static readonly Color DEFAULT_FADE_COLOUR = Color.black;

	private const float DEFAULT_FADE_SCENE_IN_DURATION  = 1;
	private const float DEFAULT_FADE_SCENE_OUT_DURATION = 1.5f;

	private const LeanTweenType DEFAULT_FADE_SCENE_IN_EASE  = LeanTweenType.easeOutSine;
	private const LeanTweenType DEFAULT_FADE_SCENE_OUT_EASE = LeanTweenType.easeInSine;

	private const float INSTANT_FADE_SCENE_OUT_DURATION = 0.01f;
	
	//-----------------------------------------------------------------------------------------
	// Inspector Variables:
	//-----------------------------------------------------------------------------------------

	[Header("Options")]

	[SerializeField] protected Color fadeColour = DEFAULT_FADE_COLOUR;
	
	[ApacheSpace]
	
	[SerializeField] protected float fadeSceneInDuration      = DEFAULT_FADE_SCENE_IN_DURATION;
	[SerializeField] protected LeanTweenType fadeSceneInEase  = DEFAULT_FADE_SCENE_IN_EASE;

	[ApacheSpace]
	
	[SerializeField] protected float fadeSceneOutDuration     = DEFAULT_FADE_SCENE_OUT_DURATION;
	[SerializeField] protected LeanTweenType fadeSceneOutEase = DEFAULT_FADE_SCENE_OUT_EASE;

	[ApacheSpace]

	[Tooltip("Should we automatically fade in on Start?")]
	[InspectorLabelBool]
	[SerializeField] protected bool shouldAutoFadeIn = true;

	[Tooltip("Should we anticipate and therefore prepare for a manual fade in some time after Start (for example, after loading subscenes)? " +
	         "If so, a tint will be set from which we'll expect to later fade in.")]
	[InspectorLabelBool]
	[SerializeField] protected bool shouldPrepareForManualFadeIn;

#if UNITY_EDITOR
	[Header("Dev")]

	[Tooltip("In the editor, should the first scene load not fade in? It can be annoying to have to constantly sit through fades " +
	         "every time the play button is pressed.")]
	[InspectorLabelBool]
	[SerializeField] protected bool shouldEditorFadeInFirstScene;
#endif

	//-----------------------------------------------------------------------------------------
	// Private Fields:
	//-----------------------------------------------------------------------------------------

	private bool hasTransitionedIn;

	/// <summary>Have we handled the first transition in since starting the application?</summary>
	/// <remarks>This is used to prevent fading in in the editor the first time play is hit.</remarks>
#if UNITY_EDITOR
	private static bool hasHandledFirstAppTransitionIn;
#endif

	//-----------------------------------------------------------------------------------------
	// Unity Lifecycle:
	//-----------------------------------------------------------------------------------------

	protected void Start() {

		// if we're not fading in the first scene in the editor, and this is the first transition...
#if UNITY_EDITOR
		if (!shouldEditorFadeInFirstScene && !hasHandledFirstAppTransitionIn) {

			// if we're not anticipating a manual fade, flag that it's all now dealt with.
			if (!shouldPrepareForManualFadeIn) {
				hasTransitionedIn              =
				hasHandledFirstAppTransitionIn = true;
			}

			// return regardless of whether we're manually fading in, because we don't want to set any tints.
			// N.B. if we're anticipating a manual fade in, we expect the outside world to call TransitionIn (below). Letting them always do that means they
			// don't have to manage state and not call it if not fading for the first time. So, once they call that, we'll then flag that it's all handled.
			return;
		}
#endif

		// if we're manually fading in, set a tint in anticipation for a fade in.
		if (shouldPrepareForManualFadeIn) {
			ScreenTintController.Instance.SetTint(Instance.fadeColour);
			return;
		}

		// handle auto fade in on start.
		if (shouldAutoFadeIn) {
			TransitionIn();
			return;
		}

		// if we're making it here, we're not fading in and nor are we expecting a manual fade in, so flag everything sorted.
		hasTransitionedIn = true;
#if UNITY_EDITOR
		hasHandledFirstAppTransitionIn = true;
#endif
	}

	//-----------------------------------------------------------------------------------------
	// Public Methods:
	//-----------------------------------------------------------------------------------------

	public static void TransitionIn() {

		// back out if we've already transitioned in.
		if (Instance.hasTransitionedIn) return;

		// if we're not fading in the very first scene, and this is our first transition, flag it's now dealt with and back out.
#if UNITY_EDITOR
		if (!Instance.shouldEditorFadeInFirstScene && !hasHandledFirstAppTransitionIn) {
			Instance.hasTransitionedIn     =
			hasHandledFirstAppTransitionIn = true;
			return;
		}
#endif
		
		// start a transition down from fade colour by default, if we have a screen tint.
		if (ScreenTintController.Instance != null) {
			ScreenTintController.Instance.SetTint(Instance.fadeColour);
			ScreenTintController.Instance.Fade   (Instance.fadeColour.WithAlpha(0), Instance.fadeSceneInDuration, Instance.fadeSceneInEase);
		}

		// flag we've transitioned in.
		Instance.hasTransitionedIn = true;

		// flag we've handled the first app-wide transition in, for editor non-fades on first start.
#if UNITY_EDITOR
		hasHandledFirstAppTransitionIn = true;
#endif
	}

	public static void TransitionOut(Action onComplete = null, bool isInstant = false) {

		// error if there is no instance.
		if (Instance == null) {
			Debug.LogError("No camera in this scene has a " + nameof(SceneTransitionController) + ".cs script on it. Please add the script to the main camera " +
			               "(or another camera, but only one) in order to transition between scenes.", Camera.main);
			return;
		}
		
		// if we have no screen tint, just call on complete and return.
		if (ScreenTintController.Instance == null) {
			onComplete?.Invoke();
			return;
		}

		// determine fade duration based on whether this is an instant transition.
		float fadeOutDuration = (isInstant) ? INSTANT_FADE_SCENE_OUT_DURATION : Instance.fadeSceneOutDuration;
		
		// begin the fade, noting that we must mark this as high priority so that the other screen tint operations can't cancel it.
		// N.B. if this was cancelled without the onComplete action being called, the entire scene transitioning/loading system would break given components,
		// like the SceneController, will continue to think they are mid-transition and therefore not initialise any other scene loads.
		ScreenTintController.Instance.Fade(Instance.fadeColour, fadeOutDuration, Instance.fadeSceneOutEase, onComplete, true);
	}

	//-----------------------------------------------------------------------------------------
	// Protected Methods:
	//-----------------------------------------------------------------------------------------

	protected override void OnIsDuplicateInstanceAndWillDelete() {
		base.OnIsDuplicateInstanceAndWillDelete();

		// if we find multiple SceneTransitionControllers, log an error.
		Debug.LogError("Multiple " + nameof(SceneTransitionController) + ".cs scripts detected. Do you have one on the main camera as well as one on a separate " +
		               "(possibly GUI-based) camera? Duplicate script detected on game object with name \"" + gameObject.name + "\".");

		// in the editor, stop playing and ensure we're not paused.
		// N.B. stopping in the editor prevents confusion given that this error will log after the duplicate instance is deleted in play mode,
		// making it impossible to track down where the duplicate used to be. When stopped, it can be found.
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPaused  =
		UnityEditor.EditorApplication.isPlaying = false;
#endif
	}
}