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

		// create our file path. 
		string dataPath = Application.persistentDataPath;
		string filePath = String.Format(FILE_NAME_FORMAT, dataPath, FILE_NAME);

		// create our serializer and stream. 
		XmlSerializer serializer = new XmlSerializer(typeof(GameData));
		FileStream stream = new FileStream(filePath, FileMode.Create);

		// serilize our data to our file. 
		serializer.Serialize(stream, gameData);
		stream.Close();
	}

	public static bool Load(out GameData gameData) {

		// create our file path. 
		string dataPath = Application.persistentDataPath;
		string filePath = String.Format(FILE_NAME_FORMAT, dataPath, FILE_NAME);

		// check our file exists. 
		if (File.Exists(filePath)) {

			// create our serializer and stream. 
			XmlSerializer serializer = new XmlSerializer(typeof(GameData));
			FileStream stream = new FileStream(filePath, FileMode.Open);

			// deserialize our data from our file. 
			gameData = serializer.Deserialize(stream) as GameData;
			stream.Close();

			return true;
		}

		Debug.LogWarning("Save File not found!");

		// if we haven't found the file, set default game data. 
		gameData = new GameData();
		return false; 
	}
}