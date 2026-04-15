using System;

/// <summary>Provides events which are guaranteed to be invoked before Unity Lifecycle events on all other components.</summary>
/// <remarks>The one exception is <c>SceneInit</c>, which is the first-running component and which does all the heavy lifting for this class.</remarks>
public static class PreEvents {

	//-----------------------------------------------------------------------------------------
	// Events:
	//-----------------------------------------------------------------------------------------

	// N.B. these events are deliberately not in the past tense so that they read similar to Unity events.
	// ReSharper disable InconsistentNaming

	public static event Action PreAwake;
	public static event Action PreOnEnable;
	public static event Action PreStart;
	public static event Action PreOnDisable;
	public static event Action PreOnDestroy;

	// ReSharper restore InconsistentNaming

	// N.B. for performance reasons, we don't continuously invoke Update, LateUpdate, and FixedUpdate events.
	// To add support for those, create a separate project-level script and invoke its events from SceneInit.cs.

	//-----------------------------------------------------------------------------------------
	// Event Methods:
	//-----------------------------------------------------------------------------------------

	// N.B. all 'On' methods act like event invocators but are intended to be called by the first-running component: Scene Init.

	public static void OnPreAwake()     { PreAwake    ?.Invoke(); }
	public static void OnPreOnEnable()  { PreOnEnable ?.Invoke(); }
	public static void OnPreStart()     { PreStart    ?.Invoke(); }
	public static void OnPreOnDisable() { PreOnDisable?.Invoke(); }
	public static void OnPreOnDestroy() { PreOnDestroy?.Invoke(); }
}