using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

[Serializable]
public class GameData : ISaveData
{
    public string Name;
    public string FileName => Name;

    //  在此处添加需要序列化保存的数据
    public List<string> collectedItems;
    public SerializableDictionary<string, ItemPickUpSaveData> activeItems;
    public SerializableDictionary<string, ShopSaveData> _shopKeeperDictionary;

    public CharacterSkinSaveData characterSkinSaveData;
}

[Serializable]
public struct LevelSave
{
    public int Level;
    public int Score;
    public int Star;

    public LevelSave(int level, int score, int star) : this()
    {
        Level = level;
        Score = score;
        Star = star;
    }
}

public interface ISaveData
{
    public string FileName { get; }
}

public interface ISaveable
{
    SerializableGuid Id { get; set; }
}

public interface IBind<TData> where TData : ISaveable
{
    SerializableGuid Id { get; set; }
    void Bind(TData data);
}

