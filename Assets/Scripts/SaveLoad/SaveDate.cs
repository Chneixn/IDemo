using System;
using System.Collections.Generic;
using UnityEngine;
using InventorySystem;

public class SaveDate
{
    public List<string> collectedItems;
    public SerializableDictionary<string, ItemPickUpSaveData> activeItems;
    public SerializableDictionary<string, ShopSaveData> _shopKeeperDictionary;

    // Player Date
    public CharacterSkinSaveData characterSkinSaveData;


    public SaveDate()
    {
        collectedItems = new();
        activeItems = new();
        _shopKeeperDictionary = new();

        characterSkinSaveData = new();
    }
}
