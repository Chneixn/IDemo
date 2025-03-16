using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtils;
using InventorySystem;

public abstract class IWeapon : MonoBehaviour
{
    public WeaponHolder holder;
    public GameObject visualModel;
    public void SetVisualModel(bool state)
    {
        if (visualModel == null)
        {
            Debug.Log("ViusalModel not set!");
            return;
        }
        visualModel.SetActiveSafe(state);
    }
    public abstract void ActivateWeapon();
    public abstract bool EnableWeapon();
    public abstract void DisableWeapon();
    public abstract void HandleInput(ref WeaponInput input);
}

public struct WeaponInput
{
    public bool quick1;
    public bool quick2;
    public bool quick3;
    public bool quick4;
    public bool fire;
    public bool aim;
    public bool reload;
    public bool switchLastWeapon;
    public bool switchFireMod;
    public float scrollSwitch;
}

public class WeaponHolder : MonoBehaviour
{
    public InventoryStorage playerStorage;
    [Header("Weapons")]
    public bool AutoEnableWeaponOnGet = false;
    public List<IWeapon> weapons = new();
    public Action<IWeapon> OnWeaponChanged;
    private IWeapon curWeapon;

    // 当前武器的在武器列表的位置，-1为尚未初始化
    private int curIndex = -1;
    private int lastIndex = -1;

    private Camera cam;
    public Camera Cam => cam;

    private void Awake()
    {
        if (cam == null)
            cam = GetComponentInChildren<Camera>();
    }

    void Start()
    {
        playerStorage = PlayerManager.Instance.PlayerInventory.Storage;
        IWeapon[] list = GetComponentsInChildren<IWeapon>();
        foreach (IWeapon i in list)
        {
            AddWeapon(i);
        }

        // if (weapons.Count != 0)
        // {
        //     SwitchWeaponByIndex(0);
        // }
    }

    public void SetWeaponHolderState(bool active)
    {
        if (!active)
        {
            for (int i = 0; i < weapons.Count; i++)
            {
                weapons[i].DisableWeapon();
            }
        }
        else
        {
            SwitchWeaponByIndex(0);
        }
    }

    /// <summary>
    /// 传入带有手部模型的武器预制体
    /// </summary>
    /// <param name="weapon"></param>
    public void AddWeapon(IWeapon weapon)
    {
        // 防止重复武器的加入, 可以修改
        if (weapons.Find(i => i.name == weapon.name + "(Clone)") != null) return;

        var newWeapon = Instantiate(weapon);
        weapons.Add(newWeapon);
        newWeapon.transform.SetParent(transform);
        newWeapon.holder = this;
        newWeapon.ActivateWeapon();
        if (AutoEnableWeaponOnGet) SwitchWeaponByClass(newWeapon);
        else newWeapon.SetVisualModel(false);
    }

    public void ReMoveWeapon(IWeapon weaponToRemove)
    {
        weapons.Remove(weaponToRemove);
    }

    #region 武器切换

    public void SwitchWeaponByClass(IWeapon weapon)
    {
        if (weapons.Contains(weapon))
        {
            SwitchWeaponByIndex(weapons.FindIndex(i => i == weapon));
        }
    }

    public void SwitchWeaponByIndex(int index)
    {
        // 检测请求的武器是否为当前武器
        if (index < 0 || index >= weapons.Count || curIndex == index) return;

        if (!curWeapon.EnableWeapon()) return;

        lastIndex = curIndex;
        // 关闭当前的武器
        if (curWeapon != null)
        {
            curWeapon.DisableWeapon();
            curWeapon.SetVisualModel(false);
        }

        // 激活请求的武器
        curIndex = index;
        curWeapon = weapons[curIndex];
        curWeapon.SetVisualModel(true);
        OnWeaponChanged?.Invoke(curWeapon);
    }

    /// <summary>
    /// 滚轮切换下一个武器
    /// </summary>
    public void SwitchLowerWeapon()
    {
        if (curIndex + 1 > weapons.Count - 1)
        {
            SwitchWeaponByIndex(curIndex + 1);
        }
        else SwitchWeaponByIndex(0);
    }

    /// <summary>
    /// 滚轮切换上一个武器
    /// </summary>
    public void SwitchUpperWeapon()
    {
        if (curIndex - 1 < weapons.Count - 1)
        {
            SwitchWeaponByIndex(curIndex - 1);
        }
        else SwitchWeaponByIndex(weapons.Count - 1);
    }

    /// <summary>
    /// 切换上一个武器
    /// </summary>
    public void SwitchLastWeapon()
    {
        SwitchWeaponByIndex(lastIndex);
    }
    #endregion

    /// <summary>
    /// 处理枪械操作输入
    /// </summary>
    public void ApplyInput(ref WeaponInput inputs)
    {
        if (inputs.quick1) SwitchWeaponByIndex(0);
        else if (inputs.quick2) SwitchWeaponByIndex(1);
        else if (inputs.quick3) SwitchWeaponByIndex(2);
        else if (inputs.quick4) SwitchWeaponByIndex(3);
        else if (inputs.switchLastWeapon) SwitchLastWeapon();
        else if (inputs.scrollSwitch > 0f) SwitchUpperWeapon();
        else if (inputs.scrollSwitch < 0f) SwitchLowerWeapon();

        if (curIndex != -1) curWeapon.HandleInput(ref inputs);
    }
}

