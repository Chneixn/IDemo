using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InventorySystem
{
    [CreateAssetMenu(menuName = "Inventory System/Shop System/Shop Item List")]
    public class ShopItemList : ScriptableObject
    {
        [SerializeField] private List<ShopIventoryItem> _items;

        [SerializeField] private int _maxAllowedGold;
        [Tooltip("出售价增值幅度: value * (1 + this)")]
        [SerializeField] private float _buyMarkUp;
        [Tooltip("收购价贬值幅度: value * (1 - this)")]
        [SerializeField] private float _sellMarkUp;

        public List<ShopIventoryItem> Items => _items;
        public int MaxAllowedGold => _maxAllowedGold;
        public float BuyMarkUp => _buyMarkUp;
        public float SellMarkUp => _sellMarkUp;
    }

    [System.Serializable]
    public struct ShopIventoryItem
    {
        public InventoryItemData ItemData;
        public int Amount;
    }
}