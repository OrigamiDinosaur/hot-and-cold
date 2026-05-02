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
	// Public Methods:
	//-----------------------------------------------------------------------------------------

	public static void Save(GameData gameData) {

		string dataPath = Application.persistentDataPath;
		string filePath = String.Format(FILE_NAME_FORMAT, dataPath, FILE_NAME);

		XmlSerializer serializer = new XmlSerializer(typeof(GameData));
		FileStream stream = new FileStream(filePath, FileMode.Create);
		serializer.Serialize(stream, gameData);
		stream.Close();
	}

	public static bool Load(out GameData gameData) {

		string dataPath = Application.persistentDataPath;
		string filePath = String.Format(FILE_NAME_FORMAT, dataPath, FILE_NAME);

		if (File.Exists(filePath)) {

			XmlSerializer serializer = new XmlSerializer(typeof(GameData));
			FileStream stream = new FileStream(filePath, FileMode.Open);
			gameData = serializer.Deserialize(stream) as GameData; 
			
			stream.Close();

			return true;
		}

		Debug.LogWarning("Save File not found!");

		gameData = new GameData(); 

		return false; 
	}
}