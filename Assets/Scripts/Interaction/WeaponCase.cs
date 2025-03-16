using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCase : MonoBehaviour, IInteractable
{
    public GameObject WeaponVisual;
    public IWeapon weaponPrefab;

    public Action<IInteractable> OnInteractionComplete { get; set; }

    public void EndInteraction()
    {
        OnInteractionComplete?.Invoke(this);
    }

    public bool Interact(Interactor interactor)
    {
        var holder = PlayerManager.Instance.WeaponHolder;
        holder.AddWeapon(weaponPrefab);

        EndInteraction();
        return true;
    }

}
