using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using InventorySystem;

public class ShoppingCartItem_UI : MonoBehaviour
{
    [SerializeField] private Image _itemIcon;
    [SerializeField] private TextMeshProUGUI _itemCount;

    public void SetItemAmount(int amount)
    {
        _itemCount.text = amount.ToString();
    }

    public void SetItemIcon(Sprite icon)
    {
        if (icon != null)
        {
            _itemIcon.sprite = icon;
            _itemIcon.color = Color.white;
        }
        else
        {
            _itemIcon.sprite = null;
            _itemIcon.color = Color.clear;
        }
    }
}