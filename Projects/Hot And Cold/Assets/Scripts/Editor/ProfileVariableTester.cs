using System.Collections.Generic;
using Apache.Core;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine.AddressableAssets.Initialization;
using UnityEngine;

class ProfileVariables {

	public int count;
	public List<string> names;
	public List<string> values;
	public List<string> editorValues;
	public List<string> runtimeValues;

	public ProfileVariables(List<string> inNames) {

		count = inNames.Count;
		names = inNames;
		values = new List<string>(count);
		editorValues = new List<string>(count);
		runtimeValues = new List<string>(count); 
	}

	public override string ToString() {

		string message = string.Empty;

		for (int i = 0; i < count; i++) {
			message += $"{names[i]} = '{values[i]}' -> '{editorValues[i]}' -> '{runtimeValues[i]}'\n";
		}

		return message;
	}
}

public class ProfileVariableTester {

	//-----------------------------------------------------------------------------------------
	// Editor Methods:
	//-----------------------------------------------------------------------------------------

	[MenuItem("HotAndCold/Test Profile Variable")]
	private static void TestProfileVariable() {

		AddressableAssetSettings addressableAssetSettings = AddressableAssetSettingsDefaultObject.Settings;
		AddressableAssetProfileSettings profileSettings = addressableAssetSettings.profileSettings;
		string activeProfileId = addressableAssetSettings.activeProfileId;

		ProfileVariables variables = new ProfileVariables(profileSettings.GetVariableNames());

		for (int i = 0; i < variables.count; i++) {

			string variableName = variables.names[i];
			string value = profileSettings.GetValueByName(activeProfileId, variableName);
			variables.values.Add(value);

			string editorValue = profileSettings.EvaluateString(activeProfileId, value);
			variables.editorValues.Add(editorValue); 

			string runtimeValue = AddressablesRuntimeProperties.EvaluateString(editorValue);
			variables.runtimeValues.Add(runtimeValue);
		}

		Debug.Log(variables);
	}

}