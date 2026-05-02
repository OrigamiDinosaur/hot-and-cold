using System;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public class SaveDataHandler : Singleton<SaveDataHandler> {

	//-----------------------------------------------------------------------------------------
	// Constants:
	//-----------------------------------------------------------------------------------------

	private const string FILE_NAME = "HotAndCold";
	private const string FILE_NAME_FORMAT = "{0}/{1}.save";

	//-----------------------------------------------------------------------------------------
	// Backing Fields:
	//-----------------------------------------------------------------------------------------

	private SaveData _saveData;

	//-----------------------------------------------------------------------------------------
	// Public Properties:
	//-----------------------------------------------------------------------------------------

	public static SaveData SaveData => Instance._saveData ?? (Instance._saveData = new SaveData());

	//-----------------------------------------------------------------------------------------
	// Public Methods:
	//-----------------------------------------------------------------------------------------

	public static void Save() {

		string dataPath = Application.persistentDataPath;
		string filePath = String.Format(FILE_NAME_FORMAT, dataPath, FILE_NAME);

		XmlSerializer serializer = new XmlSerializer(typeof(SaveData));
		FileStream stream = new FileStream(filePath, FileMode.Create);
		serializer.Serialize(stream, Instance._saveData);
		stream.Close();
	}

	public static bool Load() {

		string dataPath = Application.persistentDataPath;
		string filePath = String.Format(FILE_NAME_FORMAT, dataPath, FILE_NAME);

		if (File.Exists(filePath)) {

			XmlSerializer serializer = new XmlSerializer(typeof(SaveData));
			FileStream stream = new FileStream(filePath, FileMode.Open);
			Instance._saveData = serializer.Deserialize(stream) as SaveData; 
			
			stream.Close();

			return true;
		}

		Debug.LogWarning("Save File not found!");

		return false; 
	}
}