using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SaveLoadManager))]
public class SaveLoadManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        SaveLoadManager saveLoadSystem = (SaveLoadManager)target;
        string gameName = saveLoadSystem.GameData.Name;

        DrawDefaultInspector();

        if (GUILayout.Button("New Game"))
        {
            saveLoadSystem.OnNewGame();
        }

        if (GUILayout.Button("Save Game"))
        {
            saveLoadSystem.SaveGameData();
        }

        if (GUILayout.Button("Load Game"))
        {
            saveLoadSystem.LoadGameData(gameName);
        }

        if (GUILayout.Button("Delete Game"))
        {
            saveLoadSystem.DeleteSave(gameName);
        }
    }
}
