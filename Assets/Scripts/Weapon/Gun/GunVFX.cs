using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunVFX : MonoBehaviour
{
    private BaseGun gun;

    [Header("GunVFX视觉效果")]
    public ParticleSystem muzzleFlash;      //枪口火焰 （粒子效果）
    public Light muzzleFlashLight;          //枪口火焰灯光
    public GameObject bulletHolePrefab;           // 弹痕

    public void Start()
    {
        if (gun == null)
        {
            if (gameObject.TryGetComponent(out BaseGun gun))
            {
                this.gun = gun;
            }
        }

        gun.OnShot += OnShot;
        gun.OnShotFinshed += OnShotFinshed;
    }

    private void OnShotFinshed(bool isEmpty)
    {
        if (muzzleFlash != null) muzzleFlash.Stop();
    }

    private void OnShot(bool isEmpty)
    {
        if (muzzleFlash != null) muzzleFlash.Play();
    }



    /// <summary>
    /// 处理击中某物后的效果实现
    /// </summary>
    /// <param name="hit">射线返回的信息</param>
    public void HandleHitEffect(ref RaycastHit hit)
    {
        if (bulletHolePrefab == null) { Debug.LogWarning("未指定弹痕预制体！"); return; }

        GameObject spawnedDecal = Instantiate(bulletHolePrefab, hit.point, Quaternion.LookRotation(-hit.normal));
        //将贴花作为被击中物体的子物体
        spawnedDecal.transform.SetParent(hit.collider.transform);

        ////检测击中物体的Tag，传入对应贴花预制体
        //if (hit.collider.CompareTag("Wall"))
        //    SpawnDecal(hit, bulletHole);
        //检测击中的物体是否有材质
        //if (hit.collider.sharedMaterial != null)
        //{
        //    //缓存材质的命名
        //    string materialName = hit.collider.sharedMaterial.name;
        //    if (bulletHole == null) return;
        //    //判断不同材质名生成对应贴花
        //    switch (materialName)
        //    {
        //        case "Metal":
        //            {

        //            }
        //            break;
        //        default:
        //            {

        //            }
        //            break;
        //    }
        //}
    }
}
