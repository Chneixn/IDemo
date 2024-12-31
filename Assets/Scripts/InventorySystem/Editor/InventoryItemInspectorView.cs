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

    private Editor editor;
    private VisualElement itemImageHolder;
    private InventoryItemData item;
    private Toolbar labelList;
    private TextField labelText;
    private Image image;

    /// <summary>
    /// 将视图中的Inspector窗口与选择资源时显示的Inspector同步
    /// </summary>
    /// <param name="itemData"></param>
    public void UpdateSelection(InventoryItemData itemData)
    {
        Clear();

        if (itemData == null)
        {
            Debug.LogError("ItemData Missing!");
            return;
        }

        item = itemData;

        // 绑定image 如果有的话
        image = new();
        image.sprite = item.icon;
        image.scaleMode = ScaleMode.ScaleToFit;
        itemImageHolder.Add(image);

        if (item.Label.Count != 0)
        {
            foreach (string s in item.Label)
            {
                Label label = new($"{s}");
                label.AddToClassList("label-list-node");

                // 给添加的 label 增加右键菜单
                label.AddManipulator(new ContextualMenuManipulator((evt) =>
                {
                    evt.menu.AppendAction("Delete", (x) =>
                    {
                        Undo.RecordObject(item, "Remove label (Item Data)");
                        item.RemoveLabel(s);

                        UpdateSelection(item);
                    }, DropdownMenuAction.AlwaysEnabled);
                }));
                labelList.Add(label);
            }
        }
        // 显示新建标签
        labelText.visible = true;

        // 显示item的默认inspector窗口
        UnityEngine.Object.DestroyImmediate(editor);
        editor = Editor.CreateEditor(itemData);

        IMGUIContainer container = new(() =>
        {
            if (editor && editor.target)
            {
                editor.OnInspectorGUI();
            }
        });

        ScrollView view = new();
        view.Add(container);
        Add(view);
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
        labelText.visible = false;
        labelList.Clear();
        base.Clear();
    }

    public void Init(VisualElement itemImageHolder, Toolbar labelList, Button addButton, TextField labelText)
    {
        this.itemImageHolder = itemImageHolder;
        this.labelList = labelList;
        this.labelText = labelText;
        addButton.clicked += OnAddLabel;
    }

    private void OnAddLabel()
    {
        if (item == null) return;

        Undo.RecordObject(item, "Add label (ItemData)");
        if (item.AddLabel(labelText.value))
        {
            labelText.value = "";
            UpdateSelection(item);
        }
        else
            // Debug.LogWarning("Add label failed, check label name already exists or null!");
            Debug.LogWarning("添加标签失败, 检查标签是否已经存在或输入的标签为空!");
    }
}
