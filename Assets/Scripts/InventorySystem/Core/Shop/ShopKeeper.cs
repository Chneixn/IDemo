using UnityEngine;
using UnityEngine.Events;
using System;

namespace InventorySystem
{
    [RequireComponent(typeof(UniqueID))]
    public class ShopKeeper : MonoBehaviour, IInteractable
    {
        [SerializeField] private ShopItemList _shopItemsHeld;
        [SerializeField] private ShopSystem _shopSystem;

        public static UnityAction<ShopSystem, PlayerInventoryHolder> OnShopWindowRequested;

        public Action<IInteractable> OnInteractionComplete { get; set; }

        private string _id;
        private ShopSaveData _shopSaveData;

        private void Awake()
        {
            _shopSystem = new ShopSystem(_shopItemsHeld.Items.Count, _shopItemsHeld.MaxAllowedGold,
                _shopItemsHeld.BuyMarkUp, _shopItemsHeld.SellMarkUp);

            foreach (ShopIventoryItem item in _shopItemsHeld.Items)
            {
                //Debug.Log($"{item.ItemData.DisplayName}: {item.Amount}");
                _shopSystem.AddToShop(item.ItemData, item.Amount);
            }

            //实例化存档数据
            _id = GetComponent<UniqueID>().ID;
            _shopSaveData = new(_shopSystem);
        }

        #region 存档操作
        private void Start()
        {
            //注册存档数据
            if (!SaveGameManager.data._shopKeeperDictionary.ContainsKey(_id))
                SaveGameManager.data._shopKeeperDictionary.Add(_id, _shopSaveData);
        }

        private void OnEnable()
        {
            SaveLoad.OnLoadGame += LoadShopInventory;
        }

        private void OnDisable()
        {
            SaveLoad.OnLoadGame -= LoadShopInventory;
        }

        private void LoadShopInventory(SaveDate data)
        {
            //不存在存档数据，退出函数
            if (!data._shopKeeperDictionary.TryGetValue(_id, out ShopSaveData shopSaveData)) return;

            //存在则导入数据
            _shopSaveData = shopSaveData;
            _shopSystem = _shopSaveData.ShopSystem;
        }
        #endregion

        public void Interact(Interactor interactor, out bool interactSuccessful)
        {
            PlayerInventoryHolder playerInv = GameManager.Instance.PlayerInventory;

            if (playerInv)
            {
                OnShopWindowRequested?.Invoke(_shopSystem, playerInv);
                interactSuccessful = true;
                return;
            }
            else
            {
                interactSuccessful = false;
                Debug.LogError("PlayerInv not found!");
            }
        }

        public void EndInteraction()
        {

        }

    }
}

namespace InventorySystem
{
    [System.Serializable]
    public class ShopSaveData
    {
        public ShopSystem ShopSystem;
        public ShopSaveData(ShopSystem shopSystem)
        {
            this.ShopSystem = shopSystem;
        }
    }
}