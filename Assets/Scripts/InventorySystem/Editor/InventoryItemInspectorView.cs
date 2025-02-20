using InventorySystem;
using System;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine;
using UnityEditor.UIElements;

// TODO:选择物品图标后同步更新
public partial class InventoryItemInspectorView : VisualElement
{
    public new class UxmlFactory : UxmlFactory<InventoryItemInspectorView> { }

    public InventoryItemInspectorView() { }
    private VisualElement itemImageHolder;
    private ItemData item;
    private Image image;

    /// <summary>
    /// 将视图中的Inspector窗口与选择资源时显示的Inspector同步
    /// </summary>
    /// <param name="itemData"></param>
    public void UpdateSelection(ItemData itemData)
    {
        Clear();

        if (itemData == null)
        {
            Debug.LogError("ItemData Missing!");
            return;
        }

        item = itemData;

        // 绑定image 如果有的话
        image = new()
        {
            sprite = item.icon,
            scaleMode = ScaleMode.ScaleToFit
        };
        itemImageHolder.Add(image);

        // 显示item的默认inspector窗口
        var obj = new SerializedObject(itemData);
        Add(new InspectorElement(obj));
    }

    /// <summary>
    /// 在OnInspectorUpdate调用 检查icon图片更新
    /// </summary>
    public void UpdateImage()
    {
        if (item != null && image.sprite != item.icon)
        {
            image.sprite = item.icon;
        }
    }

    public new void Clear()
    {
        itemImageHolder.Clear();
        base.Clear();
    }

    public void Init(VisualElement itemImageHolder)
    {
        this.itemImageHolder = itemImageHolder;
    }
}
