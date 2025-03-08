using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InventorySystem;
using System;
using Cysharp.Threading.Tasks;

public class Grenadier : IWeapon
{
    public int Count;
    public float throwForce;
    public float pullPinDely;
    public float throwDely;
    public float timeToHolster;
    public Transform spawnPoint;
    public ItemData grenadeData;
    [Tooltip("要的是会爆的手雷")] public Grenade grenade_used_prefab;
    public AudioClip pullPinSound;
    public AudioClip throwGrenadeSound;
    public SkinnedMeshRenderer grenadeMesh;
    private bool animationRunning = false;

    public override void ActivateWeapon()
    {
        UpdateCount();
    }

    public override bool EnableWeapon()
    {
        UpdateCount();
        if (Count <= 0) return false;

        grenadeMesh.enabled = true;
        animationRunning = false;
        return true;
    }

    private void AutoUpdateCount(InventorySlot slot)
    {
        if (slot.ItemDate == grenadeData) UpdateCount();
    }

    private void UpdateCount()
    {
        Count = holder.playerInventory.GetItemCountFormInventory(grenadeData);
    }

    public override void DisableWeapon()
    {

    }

    public override void HandleInput(ref WeaponInput input)
    {
        if (input.fire && !animationRunning)
        {
            SpawnGrenade().Forget();
            animationRunning = true;
        }
    }

    public async UniTaskVoid SpawnGrenade()
    {
        await UniTask.WaitForSeconds(throwDely);
        grenadeMesh.enabled = false;
        if (grenade_used_prefab != null && spawnPoint != null)
        {
            Grenade grenade = GameObjectPoolManager.GetItem<Grenade>(grenade_used_prefab, spawnPoint.position, spawnPoint.rotation);
            grenade.GetComponent<Rigidbody>().AddForce(spawnPoint.transform.forward * throwForce, ForceMode.Impulse);
        }
        holder.SwitchLastWeapon();
    }

    public IEnumerator PlayGrenadeSound(AudioSource audioSource)
    {
        yield return new WaitForSeconds(pullPinDely);
        audioSource.clip = pullPinSound;
        audioSource.Play();
        yield return new WaitForSeconds(throwDely - pullPinDely);
        audioSource.clip = throwGrenadeSound;
        audioSource.Play();
        yield break;
    }
}
