using UnityEngine;
using UnityEngine.Events;
using System;

using InventorySystem;

public class ShopKeeper : MonoBehaviour, IInteractable
{
    [SerializeField] private ShopItemList _shopItemsHeld;
    [SerializeField] private ShopSystem _shopSystem;

    public static UnityAction<ShopSystem, PlayerInventoryHolder> OnShopWindowRequested;

    public Action<IInteractable> OnInteractionComplete { get; set; }

    public string InteractionText => $"与 {name} 交易";

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
    }

    public bool Interact(Interactor interactor)
    {
        PlayerInventoryHolder playerInv = PlayerManager.Instance.PlayerInventory;

        if (playerInv)
        {
            OnShopWindowRequested?.Invoke(_shopSystem, playerInv);
            return true;
        }
        else
        {
            Debug.LogError("PlayerInv not found!");
            return false;
        }
    }

    public void EndInteraction()
    {

    }

    public void OnHoverEnter(Interactor interactor)
    {
        
    }

    public void OnHoverExit(Interactor interactor)
    {
        
    }
}

[System.Serializable]
public class ShopSaveData
{
    public ShopSystem ShopSystem;
    public ShopSaveData(ShopSystem shopSystem)
    {
        this.ShopSystem = shopSystem;
    }
}
