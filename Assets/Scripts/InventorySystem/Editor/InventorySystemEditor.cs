using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Callbacks;

// TODO: item 建立分类，添加过滤器，分类显示物品

namespace InventorySystem
{
    public class InventorySystemEditor : EditorWindow
    {
        private static InventorySystemEditor window;
        private InventorySystemEditorSetting setting;
        private ItemDatabase database;
        private ObjectField itemDataBaseField;
        private Button createButton;
        private Button refreshButton;
        private Button setIDButton;
        private ToolbarPopupSearchField searchField;
        private List<InventoryItemData> searchItems;
        private Button jsonInputButton;
        private Button jsonOutButton;
        // private Button execlInputButton;
        // private Button execlOutButton;

        private Label debugLabel;

        private VisualElement itemListHolder;
        private ListView itemListView;
        private InventoryItemInspectorView inspectorView;

        // 子面板
        private ItemCreateEditor itemCreateEditor;
        private ItemIDEditor itemIDEditor;

        /// <summary>
        /// 打开 InventorySystemEditor 视图
        /// </summary>
        [MenuItem("CustomSystem/InventorySystemEditor")]
        public static void OpenWindow()
        {
            window = GetWindow<InventorySystemEditor>("InventorySystemEditor");
            window.minSize = new Vector2(800, 600);
            window.Focus();
        }

        /// <summary>
        /// 实现双击资源文件时打开可视化编辑视图
        /// </summary>
        /// <param name="instanceId">资源的id</param>
        /// <param name="line"></param>
        /// <returns></returns>
        [OnOpenAsset]
        public static bool OnPoenAsset(int instanceId, int line)
        {
            if (Selection.activeObject is ItemDatabase)
            {
                OpenWindow();
                return true;
            }
            return false;
        }

        public void CreateGUI()
        {
            setting = InventorySystemEditorSetting.GetOrCreateSettings();

            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            // 获取 UXML
            VisualTreeAsset visualTree = setting.editorUxml;
            visualTree.CloneTree(root);

            StyleSheet styleSheet = setting.editorStyle;
            root.styleSheets.Add(styleSheet);

            itemDataBaseField = root.Q<ObjectField>("itemDataBase-select");
            itemDataBaseField.objectType = typeof(ItemDatabase);
            itemDataBaseField.allowSceneObjects = false;
            itemDataBaseField.RegisterValueChangedCallback((i) => OnDatebaseChange());

            // 刷新视图按钮
            refreshButton = root.Q<Button>("refresh-button");
            refreshButton.clicked += OnRefresh;

            // 创建新物品按钮
            createButton = root.Q<Button>("create-button");
            createButton.clicked += OnCreate;

            // ID 设置按钮，为所有未设置ID的物品(ID为-1)自动设置ID
            setIDButton = root.Q<Button>("setID-button");
            setIDButton.clicked += SetItemID;

            // 搜索框绑定
            searchField = root.Q<ToolbarPopupSearchField>("searchField");
            searchField.RegisterValueChangedCallback((x) => OnSearchValueChange(x.newValue));

            // Json和Execl按钮绑定
            jsonInputButton = root.Q<Button>("json-input");
            jsonInputButton.clicked += InputJsonFile;

            jsonOutButton = root.Q<Button>("json-output");
            jsonOutButton.clicked += OutJsonFile;

            // execlInputButton = root.Q<Button>("execl-input");
            // execlInputButton.clicked += InputExeclFile;

            // execlOutButton = root.Q<Button>("execl-out");
            // execlOutButton.clicked += OutExeclFile;

            inspectorView = root.Q<InventoryItemInspectorView>("iteminspector-view");
            inspectorView.Init(root.Q<VisualElement>("item-Image"), root.Q<Toolbar>("label-list"), root.Q<Button>("add-label"), root.Q<TextField>("new-label-text"));

            // Item List Holder
            itemListHolder = root.Q<VisualElement>("itemList-holder");

            debugLabel = root.Q<Label>("Debug");
        }

        private void OnInspectorUpdate()
        {
            inspectorView.UpdateImage();
        }

        /// <summary>
        /// 当在资源文件夹中选择物品库时，切换至新物品库
        /// </summary>
        private void OnSelectionChange()
        {
            EditorApplication.delayCall += () =>
            {
                //获取选择对象
                ItemDatabase database = Selection.activeObject as ItemDatabase;

                if (database) itemDataBaseField.value = database;
            };
        }

        #region ItemList
        /// <summary>
        /// 刷新物品显示列表
        /// </summary>
        private void RefreshItemListView(List<InventoryItemData> items)
        {
            itemListHolder.Clear();
            itemListView = new ListView
            {
                itemsSource = items,
                makeItem = MakeListItem,
                bindItem = BindListItem
            };
            itemListView.selectionChanged += OnSelectItemInList;
            itemListView.AddToClassList("item-list-view");
            itemListHolder.Add(itemListView);
        }

        /// <summary>
        /// 在 itemList 中选中物品时
        /// </summary>
        /// <param name="selectedItems"></param>
        private void OnSelectItemInList(IEnumerable<object> selectedItems)
        {
            var enumerator = selectedItems.GetEnumerator();

            // 当选择 item 不为空时
            if (enumerator.MoveNext())
            {
                var item = enumerator.Current as InventoryItemData;

                inspectorView.UpdateSelection(item);
            }
        }

        /// <summary>
        /// 当 itemList 需要显示元素时创建 UI 元素
        /// </summary>
        /// <returns></returns>
        private VisualElement MakeListItem()
        {
            Label label = new();
            label.AddToClassList("item-list-node");
            return label;
        }

        /// <summary>
        /// itemList 中元素的数据绑定
        /// </summary>
        /// <param name="element"></param>
        /// <param name="index"></param> <summary>
        private void BindListItem(VisualElement element, int index)
        {
            if (database == null) return;

            InventoryItemData item = database.Items[index];
            Label label = element as Label;
            label.text = $"[{item.ID}] {item.displayName}";

            // The manipulator handles the right click and sends a ContextualMenuPopulateEvent to the target element.
            // The callback argument passed to the constructor is automatically registered on the target element.
            // 给添加的 label 增加右键菜单
            label.AddManipulator(new ContextualMenuManipulator((evt) =>
            {
                evt.menu.AppendAction("Delete", (x) =>
                {
                    database.DeleteItem(item);
                    OnRefresh();
                }, DropdownMenuAction.AlwaysEnabled);
                evt.menu.AppendAction("Chang ID", (x) =>
                {
                    itemIDEditor = ItemIDEditor.OpenWindow();
                    itemIDEditor.UpdateSelection(database, database.Items[index]);
                    itemIDEditor.editor = this;

                }, DropdownMenuAction.AlwaysEnabled);
            }));
        }
        #endregion

        private void OnDatebaseChange()
        {
            this.database = itemDataBaseField.value as ItemDatabase;
            OnRefresh();
        }

        private void SetItemID()
        {
            if (database == null) return;

            database.SetItemIDs();

            OnRefresh();
        }

        #region On Item Create
        /// <summary>
        /// 创建新的物品
        /// </summary>
        private void OnCreate()
        {
            if (itemCreateEditor != null)
            {
                itemCreateEditor.Focus();
            }
            else if (itemDataBaseField.value != null)
            {
                itemCreateEditor = ItemCreateEditor.OpenWindow();
                itemCreateEditor.Init(this, itemDataBaseField.value as ItemDatabase);
            }
            else
            {
                //Debug.LogWarning("请先选择一个 Item Data Base! ");
                debugLabel.text = "请先选择一个 Item Data Base! ";
            }
        }
        #endregion

        private void ChangeDatabase(ItemDatabase database)
        {
            itemDataBaseField.value = database;
        }

        /// <summary>
        /// 刷新物品列表
        /// </summary>
        public void OnRefresh()
        {
            // 清除 item 数据的窗口内容
            itemListHolder.Clear();
            inspectorView.Clear();

            if (itemIDEditor != null) itemIDEditor.Close();
            if (itemCreateEditor != null) itemCreateEditor.Close();

            // if (itemIDEditor == null && itemCreateEditor == null)
            //     Debug.Log("sub editor window is null");

            if (database != null)
            {
                database.SortItemList();
                RefreshItemListView(database.Items);
            }

            debugLabel.text = "ToolTips...";
        }

        /// <summary>
        /// 当在搜索栏进行搜索时
        /// </summary>
        /// <param name="value"></param>
        private void OnSearchValueChange(string value)
        {
            if (database == null) return;

            if (value == null || value == "")
            {
                RefreshItemListView(database.Items);
            }
            else
            {
                // 获取名字包含 value 或 ID 包含 value 的所有物品
                searchItems = database.Items.FindAll(i =>
                {
                    // contains 函数包含一个 StringComparison.OrdinalIgnoreCase 参数可以忽略大小写进行检查
                    return i.displayName.Contains(value, StringComparison.OrdinalIgnoreCase) || i.ID.ToString().Contains(value);
                });
                RefreshItemListView(searchItems);
            }
        }

        /// <summary>
        /// 检查database是否为空
        /// </summary>
        /// <returns>false为空</returns>
        private bool CheckDatabase()
        {
            if (database == null)
            {
                //Debug.Log("请先选择一个物品数据库(ItemDatabase)!");
                debugLabel.text = "请先选择一个物品数据库(ItemDatabase)!";
                return false;
            }
            return true;
        }

        #region Json处理
        /// <summary>
        /// 从Json文件导入物品数据库数据
        /// </summary>
        private void InputJsonFile()
        {
            string path = EditorUtility.OpenFilePanel("选择Json文件", Application.dataPath, "json");

            // 涉及System.IO时推荐使用try获取系统异常
            try
            {
                if (path == null) return;
                // 列表序列化需要经过中间件，放入一个Serialization结构的target列表中，因此需要对json字符串加工
                string json = File.ReadAllText(path, System.Text.Encoding.UTF8);
                json = "{\"target\":" + json + "}";
                List<ItemSaveData> itemDates = JsonUtility.FromJson<Serialization<ItemSaveData>>(json).ToList();
                ItemDatabase database = ScriptableObject.CreateInstance<ItemDatabase>();

                string fileName = Path.GetFileNameWithoutExtension(path);
                AssetDatabase.CreateAsset(database, $"Assets/InventorySystem/ItemDatabase/{fileName}.asset");

                foreach (var data in itemDates)
                {
                    var item = ScriptableObject.CreateInstance<InventoryItemData>();
                    ReadSaveDate(item, data);

                    database.AddItem(item);
                }

                // 保存创建的资源
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                Debug.Log($"Creat ItemDatabase which contain {database.Items.Count} items, in Assets/InventorySystem/ItemDatabase/{fileName}.asset", database);
                ChangeDatabase(database);
            }
            catch (System.Exception exception)
            {
                Debug.LogError($"Failed to read data from {path}. \n{exception}");
            }
        }

        /// <summary>
        /// 读取序列化数据ItemSaveData
        /// </summary>
        /// <param name="item"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private InventoryItemData ReadSaveDate(InventoryItemData item, ItemSaveData data)
        {
            item.name = $"[{data.id}]{data.display_name}";
            item.ID = data.id;
            item.displayName = data.display_name;
            item.description = data.description;
            if (data.label.Count != 0)
            {
                foreach (var s in data.label)
                {
                    item.Label.Add(s);
                }
            }
            item.max_stack_size = data.max_stack_size;
            item.value = data.value;
            if (data.icon_path != null)
                item.icon = AssetDatabase.LoadAssetAtPath<Sprite>(data.icon_path);
            if (data.prefab_path != null)
                item.item_prefab = AssetDatabase.LoadAssetAtPath<GameObject>(data.prefab_path);

            return item;
        }

        /// <summary>
        /// 导出Json文件
        /// </summary>
        private void OutJsonFile()
        {
            if (!CheckDatabase()) return;

            string path = EditorUtility.SaveFilePanel("选择需要保存Json文件的文件夹", Application.dataPath, database.name, "json");
            if (path == "") return;

            // 将所有 itemdata 转换为需要序列化的 savedata
            List<ItemSaveData> saveDates = new();
            foreach (var i in database.Items)
            {
                saveDates.Add(new ItemSaveData(i));
            }

            // 使用 JsonUtility 工具导出 Json 文件时，列表不能直接序列化，需要经过一次中间件
            string json = JsonUtility.ToJson(new Serialization<ItemSaveData>(saveDates));
            json = json.Remove(0, 10);
            json = json.Remove(json.Length - 1);

            try
            {
                File.WriteAllText(path, json);
                Debug.Log($"Save Json file to {path}.", database);
            }
            catch (System.Exception exception)
            {
                Debug.LogError($"Failed to save data to {path}. \n{exception}");
            }
        }

        #endregion

        private void OnDestroy()
        {
            if (itemIDEditor != null) itemIDEditor.Close();
            if (itemCreateEditor != null) itemCreateEditor.Close();
        }
    }

    /// <summary>
    /// 序列化 List<T> 的中间件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class Serialization<T>
    {
        [SerializeField]
        List<T> target;
        public List<T> ToList() { return target; }

        public Serialization(List<T> target)
        {
            this.target = target;
        }
    }
}
