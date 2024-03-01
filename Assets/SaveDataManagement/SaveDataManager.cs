using System;
using System.IO;
using UnityEngine;

public static class SaveDataManager
{

    private static string filePath = Application.persistentDataPath + "/savedata.json";

    public static void SaveGameData(SaveData data) {
        string jsonData = JsonUtility.ToJson(data);
        File.WriteAllText(filePath, jsonData);
        Debug.Log("Saved Data to " + filePath);
    }

    public static SaveData LoadGameData() {
        SaveData loadedData;
        if (File.Exists(filePath))
        {
            try
            {
            string jsonData = File.ReadAllText(filePath);
            loadedData = JsonUtility.FromJson<SaveData>(jsonData);
            return loadedData;
            }
            catch (Exception error)
            {
                Debug.LogError($"Failed to properly load Saved Data: {error}");
            }
        }
        loadedData = new SaveData();
        SaveGameData(loadedData);
        return loadedData;
    }
}
