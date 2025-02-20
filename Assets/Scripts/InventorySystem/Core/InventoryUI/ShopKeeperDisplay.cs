using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace InventorySystem
{
    public class ShopKeeperDisplay : MonoBehaviour
    {
        [SerializeField] private ShopSlot_UI _shopSlotPrefab;
        [SerializeField] private ShoppingCartItem_UI _shoppingCartItemPrefab;

        [SerializeField] private Button _buyTab;
        [SerializeField] private Button _sellTab;

        [Header("Shopping Cart")]
        [SerializeField] private TextMeshProUGUI _basketTotalText;
        [SerializeField] private TextMeshProUGUI _playerGoldText;
        [SerializeField] private TextMeshProUGUI _shopGoldText;
        [SerializeField] private Button _buyButton;
        [SerializeField] private TextMeshProUGUI _buyButtonText;

        [Header("Item Preview Section")]
        [SerializeField] private Transform _itemPreview;
        [SerializeField] private Image _itemPreviewSprite;
        [SerializeField] private TextMeshProUGUI _itemPreviewName;
        [SerializeField] private TextMeshProUGUI _itemPreviewDescription;

        [Header("Panel")]
        [SerializeField] private GameObject _itemListContentPanel;
        [SerializeField] private GameObject _shoppingCartContentPanel;

        private int _basketTotalValue;
        private bool _isPlayerSell;

        private ShopSystem _shopSystem;
        private PlayerInventoryHolder _playerInv;

        //记录购物车内物品数据与物品数量字典
        private Dictionary<ItemData, int> _shoppingCart = new();
        //记录购物车内物品数据与物品对应UI
        private Dictionary<ItemData, ShoppingCartItem_UI> _shoppingCartUI = new();

        public void WhenDisplayShopWindow(ShopSystem shopSystem, PlayerInventoryHolder playerInventory)
        {
            _shopSystem = shopSystem;
            _playerInv = playerInventory;

            _isPlayerSell = false;

            RefreshDisplay();
        }

        private void RefreshDisplay()
        {
            if (_buyButton != null)
            {
                _buyButtonText.text = _isPlayerSell ? "卖" : "买";
                _buyButton.onClick.RemoveAllListeners();
                if (_isPlayerSell) _buyButton.onClick.AddListener(SellItems);
                else _buyButton.onClick.AddListener(BuyItems);
            }

            ClearShopSlots();
            ClearItemPreview();

            _basketTotalText.enabled = false;
            _basketTotalText.color = Color.white;
            _basketTotalValue = 0;
            _buyButton.gameObject.SetActive(false);
            // _playerGoldText.text = $"玩家金额：{_playerInv.PrimaryInventorySystem.GetItemCountFormInventory()}";
            _shopGoldText.text = $"商家金额：{_shopSystem.AvailableGold}";

            if (_isPlayerSell) DisPlayPlayerInventory();
            else DisplayShopInventory();
        }

        /// <summary>
        /// 确认物品售出
        /// </summary>
        private void SellItems()
        {
            if (_shopSystem.AvailableGold < _basketTotalValue) return;

            foreach (var kvp in _shoppingCart)
            {
                int price = GetModifiedPrice(kvp.Key, kvp.Value, _shopSystem.SellMarkUp);
                _shopSystem.SellItem(kvp.Key, kvp.Value, price);

                // _playerInv.PrimaryInventorySystem.GainGold(price);
                _playerInv.PrimaryInventorySystem.RemoveItemsFromInventory(kvp.Key, kvp.Value, out int _);
            }

            RefreshDisplay();
        }

        /// <summary>
        /// 确认物品购买
        /// </summary>
        private void BuyItems()
        {
            // 当金额不足以购买，返回
            // if (_playerInv.PrimaryInventorySystem.Gold < _basketTotalValue) return;
            // 当库存空间不足以全部装入，返回
            if (!_playerInv.PrimaryInventorySystem.CheckInventoryRemaining(_shoppingCart)) return;

            foreach (var kvp in _shoppingCart)
            {
                _shopSystem.PurchaseItem(kvp.Key, kvp.Value);
                _playerInv.PrimaryInventorySystem.AddToInventory(kvp.Key, kvp.Value);
            }

            _shopSystem.GainGold(_basketTotalValue);
            // _playerInv.PrimaryInventorySystem.SpendGold(_basketTotalValue);

            RefreshDisplay();
        }

        private void DisplayShopInventory()
        {
            foreach (ShopSlot item in _shopSystem.ShopInventory)
            {
                if (item.ItemDate == null) continue;
                ShopSlot_UI shopSlot = Instantiate(_shopSlotPrefab, _itemListContentPanel.transform);
                shopSlot.Init(item, _shopSystem.BuyMarkUp);
            }
        }

        private void DisPlayPlayerInventory()
        {
            var itemsRecord = _playerInv.PrimaryInventorySystem.ItemsRecord; //缓存玩家库存
            if (itemsRecord.Count <= 0) return; //判定库存是否为空
            foreach (var item in itemsRecord)
            {
                ShopSlot _slot = new();
                _slot.AssignItem(item.Key, item.Value);

                ShopSlot_UI shopSlot = Instantiate(_shopSlotPrefab, _itemListContentPanel.transform);
                shopSlot.Init(_slot, _shopSystem.SellMarkUp);
            }
        }

        /// <summary>
        /// 从购物车去除物品
        /// </summary>
        /// <param name="shopSlot_UI"></param>
        public void RemoveItemFromCart(ShopSlot_UI shopSlot_UI)
        {
            //缓存物品数据和物品价格
            ItemData data = shopSlot_UI.AssignedItemSlot.ItemDate;
            int price = GetModifiedPrice(data, 1, shopSlot_UI.MarkUp);

            //UI处理
            if (_shoppingCart.ContainsKey(data))
            {
                //减少购物车字典对应物品数量
                _shoppingCart[data]--;

                //在购物车物品UI显示购买的数量
                _shoppingCartUI[data].SetItemAmount(_shoppingCart[data]);

                //当购物车字典内已无该类型物品时
                if (_shoppingCart[data] <= 0)
                {
                    _shoppingCart.Remove(data); //从购物车字典删除该物品
                    GameObject obj = _shoppingCartUI[data].gameObject; //获取购物车内物品UI
                    _shoppingCartUI.Remove(data); //从购物车UI字典删除该物品
                    Destroy(obj);
                }

                //计算购物车金额并显示
                _basketTotalValue -= price;
                UpdateBasketTotalValueText(_basketTotalValue);

                //当购物车已经没有物品时
                if (_shoppingCart.Count <= 0 && _basketTotalText.IsActive())
                {
                    _basketTotalText.enabled = false;
                    _buyButton.gameObject.SetActive(false);
                    ClearItemPreview();
                    return;
                }

                CheckCartValueVsAvailableGold();
            }
        }

        /// <summary>
        /// 增加物品进入购物车
        /// </summary>
        /// <param name="shopSlot_UI"></param>
        internal void AddItemToCart(ShopSlot_UI shopSlot_UI)
        {
            ItemData data = shopSlot_UI.AssignedItemSlot.ItemDate;

            UpdateItemPreview(shopSlot_UI);

            //计算购买物品的单价
            int price = GetModifiedPrice(data, 1, shopSlot_UI.MarkUp);

            //购物车已有该同类物品
            if (_shoppingCart.ContainsKey(data))
            {
                //增加购物车字典对应物品数量
                _shoppingCart[data]++;

                //在购物车物品UI显示购买的数量
                _shoppingCartUI[data].SetItemAmount(_shoppingCart[data]);
            }
            else
            {
                //记录购物车中的物品数据与数量
                _shoppingCart.Add(data, 1);

                //生成购物车中物品UI
                ShoppingCartItem_UI _UIObj = Instantiate(_shoppingCartItemPrefab, _shoppingCartContentPanel.transform);
                _UIObj.SetItemAmount(_shoppingCart[data]);
                _UIObj.SetItemIcon(data.icon);

                //绑定购物车中的物品数据与UI
                _shoppingCartUI.Add(data, _UIObj);
            }

            //计算购物车金额并显示
            _basketTotalValue += price;
            UpdateBasketTotalValueText(_basketTotalValue);

            //显示交易按钮和购物车金额
            if (_basketTotalValue > 0 && !_basketTotalText.IsActive())
            {
                _basketTotalText.enabled = true;
                _buyButton.gameObject.SetActive(true);
            }

            CheckCartValueVsAvailableGold();
        }

        private void UpdateItemPreview(ShopSlot_UI shopSlot_UI)
        {
            ItemData data = shopSlot_UI.AssignedItemSlot.ItemDate;

            _itemPreviewSprite.sprite = data.icon;
            _itemPreviewSprite.color = Color.white;
            _itemPreviewName.text = data.displayName;
            _itemPreviewDescription.text = data.description;

            _itemPreview.gameObject.SetActive(true);
        }

        private void ClearItemPreview()
        {
            _itemPreviewSprite.sprite = null;
            _itemPreviewSprite.color = Color.clear;
            _itemPreviewName.text = string.Empty;
            _itemPreviewDescription.text = string.Empty;

            _itemPreview.gameObject.SetActive(false);
        }

        private void ClearShopSlots()
        {
            _shoppingCart = new Dictionary<ItemData, int>();
            _shoppingCartUI = new Dictionary<ItemData, ShoppingCartItem_UI>();

            //清除Panel对象下的所有子物品
            foreach (var item in _itemListContentPanel.transform.Cast<Transform>())
            {
                Destroy(item.gameObject);
            }

            foreach (var item in _shoppingCartContentPanel.transform.Cast<Transform>())
            {
                Destroy(item.gameObject);
            }
        }

        /// <summary>
        /// 判断交易能否完成
        /// </summary>
        private void CheckCartValueVsAvailableGold()
        {
            #region 金额判定交易能否完成
            // 判断当前处于购买还是出售
            // int goldToCheck = _isPlayerSell ? _shopSystem.AvailableGold : _playerInv.PrimaryInventorySystem.Gold;
            int goldToCheck = 0;

            //判断能否支付交易金额
            if (_basketTotalValue > goldToCheck)
            {
                //交易无法完成提示
                if (_isPlayerSell) _basketTotalText.text = "商人金额不足";
                else _basketTotalText.text = $"玩家金额不足 仍需：{_basketTotalValue - goldToCheck}";
                _basketTotalText.color = Color.red;
            }
            #endregion

            #region 库存空间判断交易能否完成
            if (_isPlayerSell || _playerInv.PrimaryInventorySystem.CheckInventoryRemaining(_shoppingCart)) return;
            else
            {
                _basketTotalText.text = "你背包装不下！";
                _basketTotalText.color = Color.red;
            }
            #endregion
        }

        public void OnBuyTabPressed()
        {
            _isPlayerSell = false;
            RefreshDisplay();
        }

        public void OnSellTabPressed()
        {
            _isPlayerSell = true;
            RefreshDisplay();
        }

        public static int GetModifiedPrice(ItemData data, int amount, float markUp)
        {
            return Mathf.FloorToInt(data.value * amount * (1 + markUp));
        }

        private void UpdateBasketTotalValueText(int totalValue)
        {
            _basketTotalText.text = $"总计：{totalValue}";
        }
    }
}