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

    private AudioSource audioSource;
    private bool readToThrow = false;

    public override void ActivateWeapon()
    {
        audioSource = GetComponent<AudioSource>();
        UpdateCount();
    }

    public override bool EnableWeapon()
    {
        UpdateCount();
        if (Count <= 0) return false;

        grenadeMesh.enabled = true;
        readToThrow = true;
        return true;
    }

    private void UpdateCount()
    {
        Count = holder.playerStorage.GetItemCountFormInventory(grenadeData);
    }

    public override void DisableWeapon()
    {

    }

    public override void HandleInput(ref WeaponInput input)
    {
        if (input.fire && !readToThrow)
        {
            SpawnGrenade().Forget();
            PlayGrenadeSound().Forget();
            readToThrow = false;

            GetComponentInChildren<Animator>().SetTrigger("Throw");
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

    public async UniTaskVoid PlayGrenadeSound()
    {
        await UniTask.WaitForSeconds(pullPinDely);
        audioSource.PlayOneShot(pullPinSound);
        await UniTask.WaitForSeconds(throwDely - pullPinDely);
        if (throwGrenadeSound != null) audioSource.PlayOneShot(throwGrenadeSound);
    }
}
