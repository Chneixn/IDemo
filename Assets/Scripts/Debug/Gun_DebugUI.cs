using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Gun_DebugUI : MonoBehaviour
{
    public TextMeshProUGUI ammoText;
    private Gun gun;

    private void Start()
    {
        ammoText = GetComponentInChildren<TextMeshProUGUI>();
        PlayerManager.Instance.WeaponHolder.OnWeaponChanged += OnWeaponChange;
    }

    private void OnWeaponChange(IWeapon weapon)
    {
        if (weapon is Gun gun)
        {
            this.gun = gun;
            ammoText.enabled = true;
        }
        else
        {
            this.gun = null;
            ammoText.enabled = false;
        }
    }

    private void Update()
    {
        if (gun != null)
            ammoText.text = $"{gun.CurrentBulletsCount}/{gun.TotalBulletsLeft}";
    }
}
