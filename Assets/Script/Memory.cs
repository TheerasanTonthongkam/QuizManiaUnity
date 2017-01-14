using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class Memory : MonoBehaviour {

	private string PATH;
	private static GameData gameData;


	void Awake() {
		PATH = Application.persistentDataPath + "/saveDataQuizMania.gd";
		System.Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
	}
		
	private void Save() {
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Open (PATH, FileMode.Create);
		bf.Serialize(file, gameData);
		file.Close();
	}

	public void Load() {
		if(hasSave()) {
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(PATH, FileMode.Open);
			gameData = (GameData)bf.Deserialize(file);
			file.Close();
		}
	}

	public bool hasSave() {
		return File.Exists (PATH);
	}

	public GameData getGameData() {
		Load ();
		return gameData;
	}

	public void setGameData(GameData data) {
		gameData = data;
		Save ();
	}
}
