using System;
using System.Collections;
using System.Collections.Generic;
using InventorySystem;
using UnityEngine;

public class AmmoBox : MonoBehaviour, IInteractable
{
    public GameObject visualModel;
    public ItemData ammoData;
    public int AmmoCount = 100;
    public int AmmoCountPerInteracted = 0;

    public float InteractionCD = 0f;

    public Action<IInteractable> OnInteractionComplete { get; set; }

    public string InteractionText => $"获取{AmmoCountPerInteracted}个{ammoData.displayName}";

    public void EndInteraction()
    {
        OnInteractionComplete?.Invoke(this);
    }

    public bool Interact(Interactor interactor)
    {
        var inv = PlayerManager.Instance.PlayerInventory.Storage;
        inv.AddToInventory(ammoData, AmmoCountPerInteracted);
        AmmoCount -= AmmoCountPerInteracted;
        if (AmmoCount == 0) OnEmpty();
        EndInteraction();
        return true;
    }

    public void OnHoverEnter(Interactor interactor)
    {
        if (TryGetComponent(out Outline outline))
        {
            outline.enabled = true;
        }
    }

    public void OnHoverExit(Interactor interactor)
    {
        if (TryGetComponent(out Outline outline))
        {
            outline.enabled = false;
        }
    }

    private void OnEmpty()
    {
        gameObject.SetActive(false);
    }
}
