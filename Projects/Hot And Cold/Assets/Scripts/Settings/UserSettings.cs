public class UserSettings {

	//-----------------------------------------------------------------------------------------
	// Constants:
	//-----------------------------------------------------------------------------------------

	private const string DEFAULT_LANGUAGE_CODE = "en";

	//-----------------------------------------------------------------------------------------
	// Public Properties:
	//-----------------------------------------------------------------------------------------

	// Localisation:

	/// <summary>Should we attempt to automatically determine language code based on the system locale?</summary>
	public bool ShouldLocaliseForSystem { get; set; }

	/// <summary>Have localisation settings been determined based on the system locale?</summary>
	public bool HasLocalisedForSystem { get; set; }

	/// <summary>The current language code used for localisation.</summary>
	public string LanguageCode { get; set; } = DEFAULT_LANGUAGE_CODE;
}