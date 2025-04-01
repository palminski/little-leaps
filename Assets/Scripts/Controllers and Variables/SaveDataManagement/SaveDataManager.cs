using System;
using System.IO;
using UnityEngine;

public static class SaveDataManager
{

    private static readonly string filePath = Application.persistentDataPath + "/savedata.json";

    public static void SaveGameData(SaveData data) {
        string jsonData = JsonUtility.ToJson(data);
        File.WriteAllText(filePath, jsonData);
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

    public static void DeleteGameData() {
        SaveData loadedData;
        loadedData = new SaveData();
        SaveGameData(loadedData);
    }

    public static void AddPermanentCollectedString(String stringToAdd)
    {
        SaveData gameData = LoadGameData();
        if (gameData.permanentCollectedObjects.Contains(stringToAdd)) return;
        gameData.permanentCollectedObjects.Add(stringToAdd);
        SaveGameData(gameData);
    }

    public static void RemovePermanentCollectedString(String stringToRemove)
    {
        SaveData gameData = LoadGameData();
        if (!gameData.permanentCollectedObjects.Contains(stringToRemove)) return;
        gameData.permanentCollectedObjects.Remove(stringToRemove);
        SaveGameData(gameData);
    }
}
