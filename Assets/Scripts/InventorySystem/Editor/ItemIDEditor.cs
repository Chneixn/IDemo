using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace InventorySystem
{
    public class ItemIDEditor : EditorWindow
    {
        public InventorySystemEditor editor;
        ItemData itemData;

        IntegerField integerField;
        Button applyButton;
        Button canelButton;
        Button resetButton;
        Label logPanel;

        int resetInt;

        public static ItemIDEditor OpenWindow()
        {
            // true 属性使该窗口不可停靠
            ItemIDEditor wnd = GetWindow<ItemIDEditor>(true, "ItemIDEditor");
            wnd.minSize = new Vector2(400, 130);
            wnd.Focus();
            return wnd;
        }

        public void CreateGUI()
        {
            var setting = InventorySystemEditorSetting.GetOrCreateSettings();

            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            // 获取 UXML
            VisualTreeAsset visualTree = setting.itemIDEditorUxml;
            visualTree.CloneTree(root);

            StyleSheet styleSheet = setting.itemIDEditorStyle;
            root.styleSheets.Add(styleSheet);

            integerField = root.Q<IntegerField>("ID");
            integerField.RegisterValueChangedCallback((i) => OnValueChange(i));

            applyButton = root.Q<Button>("apply-button");
            applyButton.clicked += OnApply;

            canelButton = root.Q<Button>("canel-button");
            canelButton.clicked += OnCanel;

            resetButton = root.Q<Button>("reset-button");
            resetButton.clicked += OnReset;

            logPanel = root.Q<Label>("log-panel");
        }

        private void OnValueChange(ChangeEvent<int> i)
        {
            // 在更新中监听输入的value是否已经存在
            if (integerField.value < 0)
            {
                logPanel.text = $"Log: ID [{integerField.value}] is illegal!";
            }
            else if (usedIDs.Contains(i.newValue))
            {
                logPanel.text = $"Log: ID [{integerField.value}] already exit!";
            }
            else
            {
                logPanel.text = $"Log: ID [{integerField.value}] is OK!";
            }
        }

        // 存储已经使用过的id
        HashSet<int> usedIDs = new();

        // 获取database和itemdate
        public void UpdateSelection(ItemDatabase database, ItemData itemDate)
        {
            this.itemData = itemDate;
            resetInt = itemDate.ID;

            integerField.value = itemDate.ID;

            foreach (var i in database.Items)
            {
                if (i.ID != itemDate.ID)
                {
                    usedIDs.Add(i.ID);
                }
            }
        }

        /// <summary>
        /// 重置value
        /// </summary>
        private void OnReset()
        {
            integerField.value = resetInt;
        }

        private void OnCanel()
        {
            Close();
        }

        private void OnApply()
        {
            if (integerField.value < 0)
            {
                logPanel.text = $"Log: ID [{integerField.value}] is illegal!";
            }
            else if (!usedIDs.Contains(integerField.value))
            {
                Undo.RecordObject(itemData, "ItemDate (Change Item ID)");

                itemData.ID = integerField.value;
                itemData.name = $"[{itemData.ID}]{itemData.displayName}";

                AssetDatabase.SaveAssets();

                if (editor != null) editor.OnRefresh();

                Close();
            }
            else
            {
                logPanel.text = $"Log: ID [{integerField.value}] already exit!";
            }
        }
    }
}