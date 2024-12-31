using System;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class Knife : MonoBehaviour
{
    [Header("武器参数")]
    public float litDamage;
    public float heavyDamage;
    public float attackRange;
    public float attackCoolDown;
    public float heavyCoolDown;
    public bool allowButtonHold;
    public float timeToHolster;

    private GameObject _mainCam;
    private Camera cam;

    [Header("动画管理")]
    [SerializeField] private WeaponAnimation weaponAnimation;

    [Header("AudioSetting音频")]
    [SerializeField] private WeaponAudio weaponAudio;

    [Header("AttackEffect刀痕预制体")]
    [SerializeField] private GameObject _attackEffect;

    private Vector3 centerPoint = new(0.5f, 0.5f, 0f);
    private bool activated = false;

    public void ActivateWeapon(Camera cam, AudioSource audioSource)
    {
        if (activated) return;
        activated = true;
        this.cam = cam;
        weaponAudio = GetComponent<WeaponAudio>();
        weaponAudio.Init(audioSource);
    }

    public void Attack(float damage)
    {
        Ray _aimRay = cam.ScreenPointToRay(centerPoint);
        _aimRay.direction = _mainCam.transform.forward;

        if (Physics.Raycast(_aimRay, out RaycastHit _hitInfo, attackRange))
        {
            if (_hitInfo.collider.TryGetComponent(out IDamageable target))
            {
                target.TakeDamage(damage, DamageType.Knife, cam.transform.forward);
            }
            else if (_hitInfo.collider.CompareTag("Environments"))
            {
                SpawnDecal(_hitInfo);
            }
        }

    }

    //public void PlaySound(AudioSource audioSource)
    //{
    //    if (attackedSound != null)
    //    {
    //        audioSource.clip = attackedSound;
    //        audioSource.Play();
    //    }
    //}

    private void SpawnDecal(RaycastHit hit)
    {
        if (_attackEffect != null)
        {
            GameObject spawnedDecal = GameObjectPoolManager.SpawnObject(_attackEffect, hit.point, Quaternion.LookRotation(-hit.normal));
            spawnedDecal.transform.localPosition += new Vector3(0f, 0f, -0.02f);
            GameObjectPoolManager.DelyReturnToPoolBySeconds(spawnedDecal, 5f);
        }
    }
}