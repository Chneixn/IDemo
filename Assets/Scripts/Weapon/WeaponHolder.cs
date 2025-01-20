using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExtensionTools;

//Bug:换弹时切换武器，在切换武器后上一武器扔在换弹
//Bug:瞄准动画播放时可以开火

public interface IWeapon
{
    void EnableWeapon();
    void DisableWeapon();
}

public struct WeaponInput
{
    public bool quick0;
    public bool quick1;
    public bool quick2;
    public bool quick3;
    public bool quick4;
    public bool fire;
    public bool aim;
    public bool reload;
    public bool switchWeapon;
    public bool switchFireMod;
}

public enum WeaponType
{
    Gun,
    Knife,
    Grenade
}

public class WeaponHolder : MonoBehaviour
{
    [Header("Weapons")]
    public List<GameObject> weapons = new();
    public WeaponType CurrentWeaponType { get; private set; }

    private IWeapon currentWeapon;
    private BaseGun currentGun;
    private Knife currentKnife;
    private Grenade currentGrenade;
    public int grenadeNumber;

    // 当前武器的在武器列表的位置，-1为尚未初始化
    private int currentWeaponIndex = -1;
    private int lastWeaponIndex = -1;

    private bool endAim = false;

    private Camera mainCam;

    private void Awake()
    {
        mainCam = transform.parent.GetComponent<Camera>();
        if (mainCam == null) Debug.Log("cam missing");
    }

    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            AddWeapon(transform.GetChild(i).gameObject);
        }
        currentWeaponIndex = 1;
        SwitchWeaponByIndex(0);
    }

    public void SetWeaponHolderState(bool active)
    {
        if (!active)
        {
            for (int i = 0; i < weapons.Count; i++)
            {
                weapons[i].ISetActive(active);
            }
        }
        else
        {
            SwitchWeaponByIndex(0);
        }
    }

    public void AddWeapon(GameObject newWeapon)
    {
        weapons.Add(newWeapon);
    }

    public void ReMoveWeapon(GameObject newWeapon)
    {
        weapons.Remove(newWeapon);
    }

    #region 武器切换
    public void SwitchWeaponByIndex(int index)
    {
        // 检测请求的武器是否为当前武器
        if (index == currentWeaponIndex || index == -1) return;

        lastWeaponIndex = currentWeaponIndex;

        // 关闭当前的武器
        if (weapons[currentWeaponIndex] != null)
            weapons[currentWeaponIndex].SetActive(false);
        currentWeapon?.DisableWeapon();
        // 激活请求的武器
        weapons[index].SetActive(true);
        currentWeaponIndex = index;
        ConfirmWeaponType();
        currentWeapon.EnableWeapon();
    }

    /// <summary>
    /// 滚轮切换上一个武器
    /// </summary>
    public void SwitchNextWeapon()
    {
        if (currentWeaponIndex + 1 > weapons.Count - 1)
        {
            SwitchWeaponByIndex(currentWeaponIndex + 1);
        }
        else SwitchWeaponByIndex(0);
    }

    /// <summary>
    /// 滚轮切换下一个武器
    /// </summary>
    public void SwitchUpWeapon()
    {
        if (currentWeaponIndex - 1 < weapons.Count - 1)
        {
            SwitchWeaponByIndex(currentWeaponIndex - 1);
        }
        else SwitchWeaponByIndex(weapons.Count - 1);
    }

    /// <summary>
    /// 切换上一个武器
    /// </summary>
    public void SwitchLastWeapon()
    {
        SwitchWeaponByIndex(lastWeaponIndex);
    }

    /// <summary>
    /// 确认武器类型，切换武器脚本
    /// </summary>
    private void ConfirmWeaponType()
    {
        if (weapons[currentWeaponIndex].TryGetComponent(out BaseGun gun))
        {
            CurrentWeaponType = WeaponType.Gun;
            currentGun = gun;
            currentWeapon = gun;
            currentGun.ActivateWeapon(mainCam);
        }
        else if (weapons[currentWeaponIndex].TryGetComponent(out Knife knife))
        {
            CurrentWeaponType = WeaponType.Knife;
            currentKnife = knife;
        }
        else if (weapons[currentWeaponIndex].TryGetComponent(out Grenade grenade))
        {
            CurrentWeaponType = WeaponType.Grenade;
            currentGrenade = grenade;
            currentGrenade.Enable();
        }
        else
        {
            Debug.Log("No Weapon Type Confirm!");
            return;
        }

    }
    #endregion

    /// <summary>
    /// 处理枪械操作输入
    /// </summary>
    public void ApplyInput(ref WeaponInput inputs)
    {
        switch (CurrentWeaponType)
        {
            case WeaponType.Gun:
                {
                    if (inputs.reload) currentGun.Reload();
                    else if (inputs.fire) currentGun.Shoot();
                    else if (inputs.aim)
                    {
                        endAim = true;
                        currentGun.AimStart();
                    }
                    else if (endAim)
                    {
                        endAim = false;
                        currentGun.AimEnd();
                    }
                    else if (inputs.switchFireMod) currentGun.ChangeFireMode();
                }
                break;
            case WeaponType.Knife:
                {
                    //检查是否允许持续按下攻击键

                }
                break;
            case WeaponType.Grenade:
                {
                    //TODO:按住攻击键显示手雷轨迹
                    //TODO:松开攻击键抛出手雷

                }
                break;
        }
    }
}

