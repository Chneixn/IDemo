using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

public static class SaveLoad
{
    public static UnityAction OnSaveGame;
    public static UnityAction<SaveDate> OnLoadGame;

    private static string directory = "/SaveDate/";
    private static string fileName = "SaveGame.sav";

    public static bool Save(SaveDate data)
    {
        OnSaveGame?.Invoke();

        string dir = Application.persistentDataPath + directory;

        //将地址保存至剪切板
        GUIUtility.systemCopyBuffer = dir;

        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(dir + fileName, json);

        Debug.Log("Saveing!");

        return true;
    }

    public static bool CheckSaveDateExist(out SaveDate data)
    {
        string filePath = Application.persistentDataPath + directory + fileName;

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            data = JsonUtility.FromJson<SaveDate>(json);
            return true;
        }
        else
        {
            data = null;
            return false;
        }
    }

    public static void Load()
    {
        if (CheckSaveDateExist(out SaveDate data))
        {
            OnLoadGame?.Invoke(data);
        }
        else
        {
            Debug.LogError("Save file does not exist!");
        }
    }

    public static void DeleteSaveData()
    {
        string fullPath = Application.persistentDataPath + directory + fileName;

        if (File.Exists(fullPath)) File.Delete(fullPath);
    }
}
