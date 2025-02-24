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

[System.Serializable]
public class ShopSaveData
{
    public ShopSystem ShopSystem;
    public ShopSaveData(ShopSystem shopSystem)
    {
        this.ShopSystem = shopSystem;
    }
}
