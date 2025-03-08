using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace InventorySystem
{
    [Serializable]
    public class InventoryStorage
    {
        [SerializeField] private List<InventorySlot> slots;
        public List<InventorySlot> InventorySlots => slots;
        public int StorgeSize => slots.Count;   // 库存容量大小
        private readonly Dictionary<ItemData, int> itemsRecord = new();
        public Dictionary<ItemData, int> ItemsRecord => itemsRecord;

        public Action<InventorySlot> OnSlotChanged;
        public Action OnStorageUpdated;

        /// <summary>
        /// 创建一个插槽列表，输入所需的插槽数量
        /// </summary>
        /// <param name="size"></param>
        public InventoryStorage(int size)
        {
            slots = new List<InventorySlot>(size);

            for (int i = 0; i < size; i++)
            {
                slots.Add(new InventorySlot());
            }
        }

        private InventoryStorage(List<InventorySlot> slots)
        {
            var news = new List<InventorySlot>(slots.Count);

            for (int i = 0; i < slots.Count; i++)
            {
                news.Add(new InventorySlot(slots[i].ItemDate, slots[i].StackSize));
            }
        }

        public InventoryStorage Clone()
        {
            return new InventoryStorage(slots);
        }

        /// <summary>
        /// 添加物品进入插槽列表
        /// </summary>
        /// <param name="item"></param>
        /// <param name="amount"></param>
        /// <returns>剩余未进入库存的物品数量</returns>
        public int AddToInventory(ItemData item, int amount)
        {
            // 记录未能进库的数量
            int left = amount;

            // 优先将物品放入已存有该物品的插槽
            if (ContainItem(item, out List<InventorySlot> invSlot))
            {
                foreach (var slot in invSlot)
                {
                    if (slot.HasEnoughRoomLeftInStack(left, out int stackLeft))
                    {
                        // 该插槽装得下
                        slot.AddToStack(left);
                        OnSlotChanged?.Invoke(slot);
                        left = 0;
                        return left;
                    }
                    else
                    {
                        // 该插槽装不下
                        slot.AddToStack(stackLeft);
                        OnSlotChanged?.Invoke(slot);
                        left -= stackLeft;
                    }
                }
            }

            while (left > 0)
            {
                // 获取空插槽进行放入
                if (HasFreeSlot(out InventorySlot free))
                {
                    if (left > item.max_stack_size)
                    {
                        // 当剩余要加入物品数大于单个插槽的容量时
                        free.UpdateSlot(item, item.max_stack_size);
                        left -= item.max_stack_size;
                        OnSlotChanged?.Invoke(free);
                    }
                    else
                    {
                        // 当剩余要加入物品数小于单个插槽的容量时
                        free.UpdateSlot(item, left);
                        left = 0;
                        OnSlotChanged?.Invoke(free);
                    }
                }
                else break;
            }

            var had = GetItemCountFormInventory(item);
            UpdateItemsRecord(item, had + amount - left);
            return left;
        }

        /// <summary>
        /// 从库存中移除物品(有保护)
        /// </summary>
        /// <param name="item">要移除的物品</param>
        /// <param name="amount">要移除的数量</param>
        /// <param name="left">库存中该物品剩余数量</param>
        /// <returns>是否移除成功</returns>
        public bool RemoveItemsFromInventory(ItemData item, int amount, out int left)
        {
            left = 0;
            if (itemsRecord.TryGetValue(item, out int l)) left = l;
            else return false;  // 库存中不存在该物品

            // 库存中物品数量少于移除数，不移除
            if (left < amount) return false;
            if (amount == 0) return true;

            // 逐个插槽进行物品删除
            ContainItem(item, out var slots);
            foreach (InventorySlot slot in slots)
            {
                int size = slot.StackSize;

                if (size < amount)
                {
                    // 单插槽中存量不够移除
                    slot.RemoveFromStack(size);
                    amount -= size;
                    left -= size;
                }
                else
                {
                    // 单插槽中存量足够移除
                    slot.RemoveFromStack(amount);
                    left -= amount;
                    UpdateItemsRecord(item, left);
                    return true;
                }
                OnSlotChanged?.Invoke(slot);
            }

            return false;
        }

        /// <summary>
        /// 查询库存中是否有存储相同物品的插槽，输出存储相同物品插槽的列表
        /// </summary>
        /// <param name="itemToAdd"></param>
        /// <param name="invSlot"></param>
        /// <returns></returns>
        private bool ContainItem(ItemData itemToAdd, out List<InventorySlot> invSlot)
        {
            invSlot = InventorySlots.Where(i => i.ItemDate == itemToAdd).ToList();

            return invSlot != null;
        }

        /// <summary>
        /// 检查插槽列表中是否有空插槽，输出一个空插槽
        /// </summary>
        /// <param name="freeSlot"></param>
        /// <returns></returns>
        public bool HasFreeSlot(out InventorySlot freeSlot)
        {
            freeSlot = InventorySlots.FirstOrDefault(i => i.ItemDate == null);
            return freeSlot != null;
        }

        /// <summary>
        /// 更新物品库记录, count 为 item 的准确数量
        /// </summary>
        /// <param name="item"></param>
        /// <param name="count"></param>
        private void UpdateItemsRecord(ItemData item, int count)
        {
            //该字典仅计算所持有物品的数量，不考虑最大可堆叠数
            if (!itemsRecord.ContainsKey(item))
            {
                if (count > 0) itemsRecord.Add(item, count);
            }
            else
            {
                itemsRecord[item] = count;
            }
            OnStorageUpdated?.Invoke();
        }

        public int GetItemCountFormInventory(ItemData item)
        {
            if (item == null)
            {
                Debug.LogWarning($"获取物品数量时发生错误！传入物品为空！返回数值 0");
                return 0;
            }
            else if (itemsRecord.TryGetValue(item, out int count))
            {
                //Debug.Log($"物品[{item.displayName}]剩余{count}个");
                return count;
            }
            //Debug.Log($"该库存中不存在[{item.displayName}]!");
            return 0;
        }
    }
}