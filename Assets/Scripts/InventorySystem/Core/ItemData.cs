using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace InventorySystem
{
    [CreateAssetMenu(menuName = "Inventory System/Inventory Item")]
    public class ItemData : ScriptableObject
    {
        [ReadOnly] public int ID = -1;
        public string displayName = "None";
        public ItemType itemType = ItemType.None;
        [TextArea(4, 4)] public string description;
        public int max_stack_size = 1;
        public int value = 0;
        public Sprite icon;
        public GameObject item_prefab;

#if UNITY_EDITOR
        /// <summary>
        /// 读取序列化数据ItemSaveData
        /// </summary>
        /// <param name="item"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public void ReadSaveDate(ItemSaveData data)
        {
            name = $"[{data.id}]{data.display_name}";
            ID = data.id;
            displayName = data.display_name;
            description = data.description;
            itemType = data.itemType;
            max_stack_size = data.maxStackSize;
            value = data.value;
            if (data.iconPath != null)
                icon = AssetDatabase.LoadAssetAtPath<Sprite>(data.iconPath);
            if (data.prefabPath != null)
                item_prefab = AssetDatabase.LoadAssetAtPath<GameObject>(data.prefabPath);
        }
#endif
    }

#if UNITY_EDITOR
    [Serializable]
    public struct ItemSaveData
    {
        public int id;
        public string display_name;
        public string description;
        public ItemType itemType;
        public int maxStackSize;
        public int value;
        public string iconPath;
        public string prefabPath;

        public ItemSaveData(ItemData itemDate)
        {
            id = itemDate.ID;
            display_name = itemDate.displayName;
            description = itemDate.description;
            itemType = itemDate.itemType;
            maxStackSize = itemDate.max_stack_size;
            value = itemDate.value;
            // 以项目相对路径存储图标和预制体
            iconPath = AssetDatabase.GetAssetPath(itemDate.icon);
            prefabPath = AssetDatabase.GetAssetPath(itemDate.item_prefab);
        }
    }
#endif
}