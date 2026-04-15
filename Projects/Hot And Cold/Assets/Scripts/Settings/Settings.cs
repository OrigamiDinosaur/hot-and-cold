using System;
using Apache.Core;

/// <summary>Provides access to persisted settings and enables saving and reloading of settings.</summary>
public static class Settings {

	//-----------------------------------------------------------------------------------------
	// Events:
	//-----------------------------------------------------------------------------------------

	public static event Action Loaded;

	//-----------------------------------------------------------------------------------------
	// Private Properties:
	//-----------------------------------------------------------------------------------------

	private static SerialisedElement<AppSettings>   AppSettingsElement   => SettingsCache.GetOrCreateSetting<AppSettings>  (nameof(App));
	private static SerialisedElement<UserSettings>  UserSettingsElement  => SettingsCache.GetOrCreateSetting<UserSettings> (nameof(User));
	private static SerialisedElement<UnitySettings> UnitySettingsElement => SettingsCache.GetOrCreateSetting<UnitySettings>(nameof(Unity));

	//-----------------------------------------------------------------------------------------
	// Public Properties:
	//-----------------------------------------------------------------------------------------
	
	public static AppSettings   App   => AppSettingsElement  .Data;
	public static UserSettings  User  => UserSettingsElement .Data;
	public static UnitySettings Unity => UnitySettingsElement.Data;

	//-----------------------------------------------------------------------------------------
	// Constructors:
	//-----------------------------------------------------------------------------------------

	static Settings() {

		// init the cache.
		SettingsCache.Init();

		// subscribe to pre-Start event so we can invoke our loaded event at a convenient time.
		// N.B. we're doing this all statically and in a static constructor so we don't need to unsubscribe.
		PreEvents.PreStart += PreStart;
	}

	//-----------------------------------------------------------------------------------------
	// Unity Lifecycle:
	//-----------------------------------------------------------------------------------------

	private static void PreStart() {

		// invoke our loaded event so the outside world can safely subscribe in OnEnable.
		Loaded?.Invoke();
	}

	//-----------------------------------------------------------------------------------------
	// Public Methods:
	//-----------------------------------------------------------------------------------------

	public static void ReloadAllSettings() {
		SettingsCache.ReloadAllSettings();
		Loaded?.Invoke();
	}

	public static void SaveAllSettings() {
		SettingsCache.SaveAllSettings();
		Loaded?.Invoke();
	}

	public static void SaveUserSettings() {
		SettingsCache.SaveSetting(UserSettingsElement);
		Loaded?.Invoke();
	}

	public static void SaveUserSettingsWithNewSystemLocalisation(string languageCode) {
		User.LanguageCode = languageCode;
		User.HasLocalisedForSystem = true;
		SaveUserSettings();
	}

	public static void MarkAsChanged() {

		// N.B. we mark as changed by simply letting the outside world know the settings have reloaded.
		Loaded?.Invoke();
	}
}