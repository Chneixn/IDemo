using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace InventorySystem
{
    [CreateAssetMenu(menuName = "Inventory System/Inventory Item")]
    public class InventoryItemData : ScriptableObject
    {
        [ReadOnly] public int ID = -1;
        public string displayName = "None";
        private List<string> label = new();
        public List<string> Label => label;
        [TextArea(4, 4)] public string description;
        public int max_stack_size = 1;
        public int value = 0;
        public Sprite icon;
        public GameObject item_prefab;

        public bool AddLabel(string s)
        {
            if (s == "") return false;

            if (label.Find(i => i == s) == null)
            {
                label.Add(s);
                return true;
            }

            return false;
        }

        public bool RemoveLabel(string s)
        {
            if (s == "") return false;

            if (label.Find(i => i == s) != "")
            {
                label.Remove(s);
                return true;
            }

            return false;
        }
    }

    [Serializable]
    public struct ItemSaveData
    {
        public int id;
        public string display_name;
        public string description;
        public List<string> label;
        public int max_stack_size;
        public int value;
        public string icon_path;
        public string prefab_path;

        public ItemSaveData(InventoryItemData itemDate)
        {
            id = itemDate.ID;
            display_name = itemDate.displayName;
            description = itemDate.description;
            // 存储物品标签
            label = new();
            if (itemDate.Label.Count != 0)
            {
                // 转数组再转列表实现深拷贝
                string[] array = new string[itemDate.Label.Count];
                itemDate.Label.CopyTo(array);
                label = array.ToList();
            }
            max_stack_size = itemDate.max_stack_size;
            value = itemDate.value;
            // 以项目相对路径存储图标和预制体
            icon_path = AssetDatabase.GetAssetPath(itemDate.icon);
            prefab_path = AssetDatabase.GetAssetPath(itemDate.item_prefab);
        }
    }
}