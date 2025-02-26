using System.Collections.Generic;
using System;
using UnityEngine;
using UnityUtils;
using System.Linq;

public class SaveLoadManager : PersistentSingleton<SaveLoadManager>
{
    [SerializeField] private GameData gameData;
    public GameData GameData => gameData;
    [SerializeField] private GlobalSetting globalSetting;
    public GlobalSetting GlobalSetting
    {
        get
        {
            if (globalSetting == null)
            {
                globalSetting = ScriptableObject.CreateInstance<GlobalSetting>();
            }
            return globalSetting;
        }
    }

    private IDataService dataService;

    protected override void Awake()
    {
        base.Awake();
        // Application.persistentDataPath 只允许在 mono 周期中访问
        dataService = new FileDataService(new JsonSerializer());
    }

    public void OnNewGame()
    {
        gameData = new GameData();
    }

    public void SaveGameData() => dataService.Save(gameData);

    public void LoadGameData(string gameName)
    {
        gameData = (GameData)dataService.Load(gameName);
    }

    public void ReloadGame() => LoadGameData(gameData.Name);

    public bool IsSaveFileExist(string name)
    {
        var saves = dataService.ListSaves();
        foreach (var save in saves)
        {
            if (save == name)
            {
                return true;
            }
        }
        return false;
    }

    public void DeleteSave(string gameName) => dataService.Delete(gameName);

    public void SaveSetting()
    {
        dataService.Save(globalSetting);
    }

    public void LoadSetting()
    {
        dataService.Load(globalSetting.FileName);
    }

    public void ResetSetting()
    {
        var newset = ScriptableObject.CreateInstance<GlobalSetting>();
        newset.SkipTutorial = globalSetting.SkipTutorial;
        globalSetting = newset;
    }

    void Bind<T, TData>(TData data) where T : MonoBehaviour, IBind<TData> where TData : ISaveable, new()
    {
        var entity = FindObjectsByType<T>(FindObjectsSortMode.None).FirstOrDefault();
        if (entity != null)
        {
            if (data == null)
            {
                data = new TData { Id = entity.Id };
            }
            entity.Bind(data);
        }
    }

    void Bind<T, TData>(List<TData> datas) where T : MonoBehaviour, IBind<TData> where TData : ISaveable, new()
    {
        var entities = FindObjectsByType<T>(FindObjectsSortMode.None);

        foreach (var entity in entities)
        {
            var data = datas.FirstOrDefault(d => d.Id == entity.Id);
            if (data == null)
            {
                data = new TData { Id = entity.Id };
                datas.Add(data);
            }
            entity.Bind(data);
        }
    }
}