using System.IO;
using UnityEngine;

namespace Apache.Core {
	public static class SettingsUtils {

		//-----------------------------------------------------------------------------------------
		// Public Methods:
		//-----------------------------------------------------------------------------------------

		public static void OpenSettingsDirectoryInExplorer() {
			string settingsPath = Path.Combine(Application.dataPath, SettingsCache.DEFAULT_RELATIVE_DIRECTORY_PATH);
			Application.OpenURL(settingsPath);
		}
	}
}