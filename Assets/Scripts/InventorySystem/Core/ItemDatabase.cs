using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace InventorySystem
{
    [Serializable]
    [CreateAssetMenu(menuName = "Inventory System/Item Database")]
    public class ItemDatabase : ScriptableObject
    {
        [SerializeField]
        private List<ItemData> items = new();
        public List<ItemData> Items => items;

        public ItemData GetItem(int id)
        {
            return items.Find(i => i.ID == id);
        }

        public List<ItemData> GetItemsByType(ItemType type)
        {
            return items.FindAll(i => i.itemType == type);
        }

#if UNITY_EDITOR
        public void SetItemIDs()
        {
            // 假设ID应该是从0开始的连续整数  
            int nextAvailableID = 0;
            HashSet<int> usedIDs = new();
            List<ItemData> notSetIDs = new();

            // 遍历整个列表，记录已有ID和未设置ID的集合
            foreach (var item in items)
            {
                if (item.ID > -1)
                {
                    // 记录已使用的ID  
                    usedIDs.Add(item.ID);
                }
                else
                {
                    notSetIDs.Add(item);
                }
            }

            foreach (var item in notSetIDs)
            {
                // 循环递增寻找一个未使用的ID  
                while (usedIDs.Contains(nextAvailableID))
                {
                    nextAvailableID++;
                }

                // 分配ID
                item.ID = nextAvailableID++;
            }

            // 在列表内部依据ID进行升序排序(需要降序时交换x和y的位置)
            items.Sort((x, y) => x.ID.CompareTo(y.ID));
        }

        public ItemData CreateItem()
        {
            ItemData item = ScriptableObject.CreateInstance<ItemData>();
            // item.guid = GUID.Generate().ToString();

            Undo.RecordObject(this, "Item DataBase (CreateItem)");
            items.Add(item);

            if (!Application.isPlaying)
            {
                AssetDatabase.AddObjectToAsset(item, this);
            }
            else Debug.LogWarning("Crete item in Play Mode, item won't save to asset!");

            Undo.RegisterCreatedObjectUndo(item, "Item DataBase (CreateItem)");

            AssetDatabase.SaveAssets();
            return item;
        }

        public void DeleteItem(ItemData item)
        {
            Undo.RecordObject(this, "Item DataBase (DeleteItem)");

            items.Remove(item);

            Undo.DestroyObjectImmediate(item);

            AssetDatabase.SaveAssets();
        }

        public void AddItem(ItemData item)
        {
            items.Add(item);
            AssetDatabase.AddObjectToAsset(item, this);
        }

        public void SortItemList()
        {
            items.Sort((x, y) => x.ID.CompareTo(y.ID));
        }

#endif
    }
}
