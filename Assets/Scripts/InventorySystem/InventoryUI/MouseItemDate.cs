using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using TMPro;
using InventorySystem;

public class MouseItemDate : MonoBehaviour
{
    public Image ItemSprite;
    public TextMeshProUGUI ItemCount;
    public InventorySlot AssignedInventorySlot;

    [SerializeField] private Transform _dropTranfrom;

    private void Awake()
    {
        ItemSprite.color = Color.clear;
        ItemSprite.preserveAspect = true;
        ItemCount.text = string.Empty;

        if (_dropTranfrom == null) Debug.LogWarning("ObjectGrabPos not found!");
    }

    /// <summary>
    /// 更换鼠标插槽中的数据
    /// </summary>
    /// <param name="invSlot"></param>
    public void UpdateMouseSlot(InventorySlot invSlot)
    {
        AssignedInventorySlot.AssignItem(invSlot);
        UpdateMouseSlot();
    }

    /// <summary>
    /// 刷新鼠标插槽中的数据
    /// </summary>
    public void UpdateMouseSlot()
    {
        ItemSprite.sprite = AssignedInventorySlot.ItemDate.icon;
        if (AssignedInventorySlot.StackSize > 1) ItemCount.text = AssignedInventorySlot.StackSize.ToString();
        else ItemCount.text = string.Empty;
        ItemSprite.color = Color.white;
    }

    private void Update()
    {
        // TODO: 支持控制器操作

        if (AssignedInventorySlot.ItemDate != null)
        {
            transform.position = Mouse.current.position.ReadValue();

            if (Mouse.current.leftButton.wasPressedThisFrame && !IsPointerOverUIObject())
            {
                // 丢弃物品功能
                // if (AssignedInventorySlot.ItemDate.item_prefab != null)
                //     GameObjectPoolManager.SpawnObject(AssignedInventorySlot.ItemDate.item_prefab, _dropTranfrom.position, Quaternion.identity);

                if (AssignedInventorySlot.StackSize > 1)
                {
                    //TODO:一次性丢弃鼠标插槽的物品
                    AssignedInventorySlot.RemoveFromStack(1);
                    UpdateMouseSlot();
                }

                ClearSlot();
            }
        }
    }

    public void ClearSlot()
    {
        AssignedInventorySlot.ClearSlot();
        ItemCount.text = string.Empty;
        ItemSprite.color = Color.clear;
        ItemSprite.sprite = null;
    }

    /// <summary>
    /// 判定鼠标是否点击UI物体
    /// </summary>
    /// <returns></returns>
    public static bool IsPointerOverUIObject()
    {
        PointerEventData eventDateCurrentPosition = new(EventSystem.current);
        eventDateCurrentPosition.position = Mouse.current.position.ReadValue();
        List<RaycastResult> results = new();
        EventSystem.current.RaycastAll(eventDateCurrentPosition, results);
        return results.Count > 0;
    }
}
