using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExtensionTools;

public enum SkinType
{
    MaleBody,
    FemaleBody,
    MaleArmor,
    FemaleArmor,
    Hat,
    Face,
    GhillieSuit,
    Hair
}

public enum Gender
{
    Female, Male
}

[System.Serializable]
public struct CharacterSkinSaveData
{
    public Gender gender;
    public List<SkinSaveData> skinSaveList;

    public CharacterSkinSaveData(Gender gender, List<SkinSaveData> skinSaveList)
    {
        this.gender = gender;
        this.skinSaveList = skinSaveList;
    }
}

[System.Serializable]
public struct SkinSaveData
{
    public SkinType skinType;
    public string skinName;
}

public class CharacterCustomization : MonoBehaviour
{
    [SerializeField] private GameObject characterModel;
    [SerializeField] private Gender gender = Gender.Male;
    public Gender Gender => gender;
    [SerializeField] private SkinLocation[] skinLocations;
    private CharacterSkinSaveData characterSkinSaveData;
    private List<SkinSaveData> skinSaveList;

    [System.Serializable]
    private class SkinLocation
    {
        public SkinType skinType;
        public GameObject[] skinItems;
        public int index;
        public Transform skinParent;
    }

    private void Awake()
    {
        //SaveLoad.OnLoadGame += LoadSkinData;
        //SaveLoad.OnSaveGame += SaveSkinData;

        //skinSaveList = new();
        //characterSkinSaveData = SetDefaultCharacterSkin(skinLocations);
    }

    private void Start()
    {
        CameraController.Instance.OnCamStateChange += ChangeModelVisibility;
    }

    private void OnDisable()
    {
        //SaveLoad.OnLoadGame -= LoadSkinData;
        //SaveLoad.OnSaveGame -= SaveSkinData;

        if (CameraController.Instance != null)
            CameraController.Instance.OnCamStateChange -= ChangeModelVisibility;
    }

    public void ChangeModelVisibility(CamState camState)
    {
        switch (camState)
        {
            case CamState.FPS:
                {
                    characterModel.ISetActive(false);
                }
                break;
            case CamState.TPS:
                {
                    characterModel.ISetActive(true);
                }
                break;
        }
    }

    //private void LoadSkinData(SaveDate data)
    //{
    //    //时间复杂度O(n^2)
    //    characterSkinSaveData = data.characterSkinSaveData;
    //    for (int i = 0; i < characterSkinSaveData.skinSaveList.Count - 1; i++)
    //    {
    //        foreach (SkinLocation sl in skinLocations)
    //        {
    //            if (sl.skinType == characterSkinSaveData.skinSaveList[i].skinType)
    //            {
    //                sl.skinParent.Find(characterSkinSaveData.skinSaveList[i].skinName).gameObject.ISetActive(true);
    //                Debug.Log($"Found{characterSkinSaveData.skinSaveList[i].skinName}");
    //                break;
    //            }
    //        }
    //    }
    //}

    //private void SaveSkinData()
    //{
    //    SaveGameManager.data.characterSkinSaveData = characterSkinSaveData;
    //}

    //public void SetGender(Gender gender)
    //{
    //    this.gender = gender;
    //    foreach (SkinLocation sl in skinLocations)
    //    {
    //        if (sl.skinType == SkinType.FemaleBody)
    //        {
    //            sl.skinParent.gameObject.ISetActive(gender == Gender.Female);
    //        }
    //        if (sl.skinType == SkinType.MaleBody)
    //        {
    //            sl.skinParent.gameObject.ISetActive(gender == Gender.Male);
    //        }
    //    }
    //}

    //public void ChangeSkin(SkinType skinType)
    //{
    //    foreach (SkinLocation sl in skinLocations)
    //    {
    //        if (sl.skinType == skinType)
    //        {
    //            if (!sl.skinParent.gameObject.activeSelf) sl.skinParent.gameObject.ISetActive(true);
    //            sl.skinParent.GetChild(sl.index).gameObject.ISetActive(false);
    //            sl.index++;
    //            if (sl.index > sl.skinParent.childCount - 1) sl.index = 0;
    //            sl.skinParent.GetChild(sl.index).gameObject.ISetActive(true);
    //            return;
    //        }
    //    }
    //}

    //private CharacterSkinSaveData SetDefaultCharacterSkin(SkinLocation[] skinLocations)
    //{
    //    CharacterSkinSaveData data = new(gender, skinSaveList);
    //    foreach (var sl in skinLocations)
    //    {
    //        SkinSaveData ssd = new();
    //        ssd.skinType = sl.skinType;
    //        if (sl.skinType == SkinType.MaleBody)
    //        {
    //            ssd.skinName = sl.skinParent.GetChild(0).name;
    //            sl.skinParent.GetChild(0).gameObject.ISetActive(gender == Gender.Male);
    //        }
    //        else if (sl.skinType == SkinType.FemaleBody)
    //        {
    //            ssd.skinName = sl.skinParent.GetChild(0).name;
    //            sl.skinParent.GetChild(0).gameObject.ISetActive(gender == Gender.Female);
    //        }
    //        else
    //        {
    //            ssd.skinName = sl.skinParent.GetChild(0).name;
    //            sl.skinParent.GetChild(0).gameObject.ISetActive(true); //其余第一个子物体为空物体
    //        }

    //        data.skinSaveList.Add(ssd);
    //    }

    //    return data;
    //}

}