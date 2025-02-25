using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "New GlobalSetting", menuName = "ScriptableObjects/New GlobalSetting")]
public class GlobalSetting : ScriptableObject, ISaveData
{
    public string FileName => "Setting";
    public bool SkipTutorial = false;

    public float MusicVolume = 0f;
    public float SFXVolume = 0f;
}
