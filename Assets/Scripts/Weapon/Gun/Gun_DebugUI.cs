using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Gun_DebugUI : MonoBehaviour
{
    public TextMeshProUGUI ammoText;
    public BaseGun gun;

    private void Start()
    {
        ammoText = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (gun != null)
        {
            ammoText.enabled = true;
            ammoText.text = $"{gun.CurrentBulletsCount}/{gun.TotalBulletsLeft}";
        }
        else
        {
            ammoText.enabled = false;
        }
    }
}
