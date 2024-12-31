using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace InventorySystem
{
    [System.Serializable]
    public class ShopSystem
    {
        [SerializeField] private List<ShopSlot> _shopInventory;
        [SerializeField] private int _availableGold;
        [SerializeField] private float _buyMarkUp;
        [SerializeField] private float _sellMarkUp;

        public List<ShopSlot> ShopInventory => _shopInventory;
        public int AvailableGold => _availableGold;
        public float BuyMarkUp => _buyMarkUp;
        public float SellMarkUp => _sellMarkUp;

        public ShopSystem(int size, int gold, float buyMarkUp, float sellMarkUp)
        {
            _availableGold = gold;
            _buyMarkUp = buyMarkUp;
            _sellMarkUp = sellMarkUp;

            SetShopSize(size);
        }

        private void SetShopSize(int size)
        {
            _shopInventory = new List<ShopSlot>();

            for (int i = 0; i < size; i++)
            {
                _shopInventory.Add(new ShopSlot());
            }
        }

        /// <summary>
        /// 向商店库存增加物品
        /// </summary>
        /// <param name="data"></param>
        /// <param name="amount"></param>
        public void AddToShop(InventoryItemData data, int amount)
        {
            if (ContainItem(data, out ShopSlot shopSlot))
            {
                //已有相同商品，增加商品数量
                shopSlot.AddToStack(amount);
            }
            else
            {
                //无相同商品，则创建新商店库存插槽，传入商品数据
                ShopSlot freeSlot = GetFreeSlot();
                freeSlot.AssignItem(data, amount);
            }
        }

        private ShopSlot GetFreeSlot()
        {
            ShopSlot freeSlot = _shopInventory.FirstOrDefault(i => i.ItemDate == null);

            if (freeSlot == null)
            {
                freeSlot = new ShopSlot();
                _shopInventory.Add(freeSlot);
            }

            return freeSlot;
        }


        /// <summary>
        /// 判断商店库存中是否有相同物品，输出商店物品插槽
        /// </summary>
        /// <param name="itemToAdd"></param>
        /// <param name="shopSlot"></param>
        /// <returns></returns>
        public bool ContainItem(InventoryItemData itemToAdd, out ShopSlot shopSlot)
        {
            shopSlot = _shopInventory.Find(i => i.ItemDate == itemToAdd);

            return shopSlot != null;
        }

        /// <summary>
        /// 从商店库存提取物品
        /// </summary>
        /// <param name="data">物品数据</param>
        /// <param name="amount">物品数量</param>
        public void PurchaseItem(InventoryItemData data, int amount)
        {
            if (!ContainItem(data, out ShopSlot slot)) return;

            slot.RemoveFromStack(amount);
        }

        public void SellItem(InventoryItemData data, int amount, int price)
        {
            AddToShop(data, amount);
            SpendGold(price);
        }

        public void SpendGold(int goldToSpend)
        {
            _availableGold -= goldToSpend;
        }

        public void GainGold(int goldToAdd)
        {
            _availableGold += goldToAdd;
        }
    }
}