using System;
using System.Collections;
using System.Collections.Generic;
using InventorySystem;
using UnityEngine;

public class AmmoBox : MonoBehaviour, IInteractable
{
    public GameObject visualModel;
    public ItemData ammoData;
    public int AmmoCount = 0;

    public float InteractionCD = 0f;

    public Action<IInteractable> OnInteractionComplete { get; set; }

    public void EndInteraction()
    {
        OnInteractionComplete?.Invoke(this);
    }

    public bool Interact(Interactor interactor)
    {
        var inv = PlayerManager.Instance.PlayerInventory.Storage;
        inv.AddToInventory(ammoData, AmmoCount);
        if (AmmoCount == 0) OnEmpty();
        EndInteraction();
        return true;
    }

    private void OnEmpty()
    {

    }
}
