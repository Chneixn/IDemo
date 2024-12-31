using UnityEngine;

public class SaveGameManager : MonoBehaviour
{
    public static SaveDate data;

    private void Awake()
    {
        data = new SaveDate();
        SaveLoad.OnLoadGame += LoadData;
    }

    #region 对存档处理
    public void DeleteData()
    {
        SaveLoad.DeleteSaveData();
    }

    public static void SaveData()
    {
        var saveData = data;

        SaveLoad.Save(saveData);
    }

    public void LoadData(SaveDate _data)
    {
        data = _data;
    }

    public static void TryLoadData()
    {
        SaveLoad.Load();
    }
    #endregion

}
