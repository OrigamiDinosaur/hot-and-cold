public class UnitySettings {

	//-----------------------------------------------------------------------------------------
	// Public Properties:
	//-----------------------------------------------------------------------------------------

	// Graphics:

	/// <summary>Should VSync be enabled? VSync is used to keep the game refresh rate synchronised with that of the display.</summary>
	public bool EnableVSync { get; protected set; } = true;

	// Input:

	/// <summary>Should the mouse cursor be visible while hovering over the application window?</summary>
	public bool ShowCursor { get; protected set; }

	/// <summary>Should keyboard shortcuts such as accessing settings or jumping between scenes be enabled? The escape key will still quit the app.</summary>
	public bool EnableKeyboardShortcuts { get; protected set; }
#if (UNITY_ANDROID || UNITY_IPHONE) && !UNITY_EDITOR
		= false;
#else
		= true;
#endif
}