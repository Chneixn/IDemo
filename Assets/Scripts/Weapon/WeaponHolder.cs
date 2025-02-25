using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtils;
//Bug:换弹时切换武器，在切换武器后上一武器扔在换弹
//Bug:瞄准动画播放时可以开火

public abstract class IWeapon : MonoBehaviour
{
    public GameObject visualModel;
    /// <summary>
    /// 控制武器模型显示
    /// </summary>
    /// <param name="state"></param>
    public void SetVisualModel(bool state)
    {
        if (visualModel == null)
        {
            Debug.Log("ViusalModel not set!");
            return;
        }
        visualModel.SetActiveSafe(state);
    }
    public abstract void EnableWeapon(Camera cam);
    public abstract void DisableWeapon();
    public abstract void HandleInput(ref WeaponInput input);
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

public class WeaponHolder : MonoBehaviour
{
    [Header("Weapons")]
    public List<IWeapon> weapons = new();

    private IWeapon currentWeapon;

    // 当前武器的在武器列表的位置，-1为尚未初始化
    private int curIndex = -1;
    private int lastIndex = -1;

    private Camera cam;

    private void Awake()
    {
        if (cam == null)
            cam = GetComponentInChildren<Camera>();
    }

    void Start()
    {
        var list = GetComponentsInChildren<IWeapon>();
        foreach (var i in list)
        {
            weapons.Add(i);
            i.SetVisualModel(false);
        }

        if (weapons.Count != 0)
        {
            curIndex = 0;
            lastIndex = 0;
            currentWeapon = weapons[0];
            currentWeapon.EnableWeapon(cam);
        }
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

    public void AddWeapon(IWeapon newWeapon)
    {
        weapons.Add(newWeapon);
    }

    public void ReMoveWeapon(IWeapon weaponToRemove)
    {
        weapons.Remove(weaponToRemove);
    }

    #region 武器切换
    public void SwitchWeaponByIndex(int index)
    {
        // 检测请求的武器是否为当前武器
        if (curIndex < 0 || curIndex >= weapons.Count || curIndex == index) return;
        lastIndex = curIndex;
        // 关闭当前的武器
        currentWeapon?.DisableWeapon();
        // 激活请求的武器
        curIndex = index;
        weapons[index].EnableWeapon(cam);
    }

    /// <summary>
    /// 滚轮切换上一个武器
    /// </summary>
    public void SwitchNextWeapon()
    {
        if (curIndex + 1 > weapons.Count - 1)
        {
            SwitchWeaponByIndex(curIndex + 1);
        }
        else SwitchWeaponByIndex(0);
    }

    /// <summary>
    /// 滚轮切换下一个武器
    /// </summary>
    public void SwitchUpWeapon()
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
        currentWeapon?.HandleInput(ref inputs);
    }
}

