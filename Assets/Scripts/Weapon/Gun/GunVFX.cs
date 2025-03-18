using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameObjectPool;

public class GunVFX : MonoBehaviour
{
    private Gun gun;

    [Header("GunVFX视觉效果")]
    public ParticleSystem muzzleFlash;
    public Light flashLight;
    public float flashLightTime = 0.5f;
    public IPoolableParticleSystem hitParticle;
    public float hitParticleTime = 3f;
    public GameObject bulletHolePrefab;

    private float timer = 0f;

    public void Start()
    {
        if (gun == null)
        {
            if (gameObject.TryGetComponent(out Gun gun))
            {
                this.gun = gun;
            }
        }

        gun.OnShot += OnShot;
        gun.OnHit += OnHit;
    }

    private void OnHit(RaycastHit hit)
    {
        if (bulletHolePrefab != null)
        {
            var obj = Instantiate(bulletHolePrefab, hit.point, Quaternion.LookRotation(-hit.normal));
            obj.transform.localPosition -= hit.normal * 0.01f; // 贴花偏移, 避免重叠时产生闪烁
        }

        if (hitParticle != null)
        {
            var obj = GameObjectPoolManager.GetItem<IPoolableParticleSystem>(hitParticle, hit.point, Quaternion.identity);
            obj.particle.Play();
            TimerManager.CreateTimeOut(hitParticleTime, () => GameObjectPoolManager.RecycleItem(obj));
        }
    }

    private void OnShot(bool isEmpty)
    {
        if (muzzleFlash != null) muzzleFlash.Play();
        if (flashLight != null)
        {
            flashLight.enabled = true;
            timer = flashLightTime;
        }
    }

    void Update()
    {
        // 延时关闭灯光
        if (timer > 0f && flashLight != null)
        {
            if (flashLight.enabled)
            {
                timer -= Time.deltaTime;
                if (timer <= 0f) flashLight.enabled = false;
            }
        }
    }

}
