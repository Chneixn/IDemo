using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Callbacks;

// TODO: item 建立分类，分类显示物品

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
        private List<ItemData> searchItems;
        private Button jsonInputButton;
        private Button jsonOutButton;
        private Button execlInputButton;
        private Button execlOutButton;
        private TextField execlPathField;
        private Button viewExeclPathButton;
        private TextField databasePathField;
        private Button viewDatabasePathButton;

        // tip 提示
        private Label tipLabel;

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

        #region On Create Editor
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

            execlInputButton = root.Q<Button>("execl-input");
            execlInputButton.clicked += InputExeclFile;

            execlOutButton = root.Q<Button>("execl-output");
            execlOutButton.clicked += OutExeclFile;

            execlPathField = root.Q<TextField>("execl-path");
            execlPathField.value = setting.execlPath;
            execlPathField.RegisterValueChangedCallback((x) => OnExeclPathValueChange(x.newValue));
            viewExeclPathButton = root.Q<Button>("view-execl-path");
            viewExeclPathButton.clicked += OnViewExeclPath;

            databasePathField = root.Q<TextField>("database-path");
            databasePathField.value = setting.newDatabasePath;
            databasePathField.RegisterValueChangedCallback((x) => OnDatabasePathValueChange(x.newValue));
            viewDatabasePathButton = root.Q<Button>("view-database-path");
            viewDatabasePathButton.clicked += OnViewDatabasePath;

            inspectorView = root.Q<InventoryItemInspectorView>("iteminspector-view");
            inspectorView.Init(root.Q<VisualElement>("item-Image"));

            // Item List Holder
            itemListHolder = root.Q<VisualElement>("itemList-holder");

            tipLabel = root.Q<Label>("Debug");

            if (Selection.activeObject is ItemDatabase)
            {
                itemDataBaseField.value = Selection.activeObject;
            }
        }
        #endregion

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
        private void RefreshItemListView(List<ItemData> items)
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
                var item = enumerator.Current as ItemData;

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

            ItemData item = database.Items[index];
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
                ShowTip("请先选择一个 Item Data Base! ", Color.yellow);
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

            ShowTip("ToolTips...", Color.white, false);
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

        private void OnViewExeclPath()
        {
            string path = EditorUtility.OpenFolderPanel("选择Execl存储文件夹", Application.dataPath, "");
            if (!string.IsNullOrEmpty(path))
            {
                execlPathField.value = path;
            }
        }

        private void OnExeclPathValueChange(string newValue) => setting.execlPath = newValue;

        private void OnDatabasePathValueChange(string newValue) => setting.newDatabasePath = newValue;

        private void OnViewDatabasePath()
        {
            string path = EditorUtility.OpenFolderPanel("选择Database存储文件夹", Application.dataPath, "");
            if (!string.IsNullOrEmpty(path))
            {
                databasePathField.value = path.Substring(Application.dataPath.Length - 6);
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
                ShowTip("请先选择一个物品数据库(ItemDatabase)!", Color.yellow);
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
            if (path == null || path == "") return;
            // 涉及System.IO时推荐使用try获取系统异常
            try
            {
                // 列表序列化需要经过中间件，放入一个Serialization结构的target列表中，因此需要对json字符串加工
                string json = File.ReadAllText(path, System.Text.Encoding.UTF8);
                List<ItemSaveData> itemDates = JsonUtility.FromJson<SerializableList<ItemSaveData>>(json).ToList();

                CreateAssetFromSaveData(ref itemDates, path);
            }
            catch (System.Exception exception)
            {
                ShowTip($"Failed to read data from {path}. \n{exception}", Color.red, false);
                Debug.LogError($"Failed to read data from {path}. \n{exception}");
            }
        }

        /// <summary>
        /// 导出Json文件
        /// </summary>
        private void OutJsonFile()
        {
            if (!CheckDatabase()) return;

            string path = EditorUtility.SaveFilePanelInProject("选择需要保存Json文件的文件夹", database.name, "json", "");
            if (path == "") return;

            // 将所有 itemdata 转换为需要序列化的 savedata
            List<ItemSaveData> saveDates = new();
            foreach (ItemData i in database.Items)
                saveDates.Add(new ItemSaveData(i));

            // 使用 JsonUtility 工具导出 Json 文件时，列表不能直接序列化，需要经过一次中间件
            string json = JsonUtility.ToJson(new SerializableList<ItemSaveData>(saveDates), true);

            try
            {
                File.WriteAllText(path, json);
                ShowTip($"Save Json file to {path}.", Color.yellow);
            }
            catch (System.Exception exception)
            {
                ShowTip($"Failed to save data to {path}. \n{exception}", Color.red, false);
                Debug.LogError($"Failed to save data to {path}. \n{exception}");
            }
        }

        #endregion

        #region Execl处理

        private void InputExeclFile()
        {
            string path = EditorUtility.OpenFilePanel("选择表格文件", setting.execlPath, "xlsx");
            if (path == null || path == "") return;
            // 涉及System.IO时推荐使用try获取系统异常
            try
            {
                // 获取execl文件
                Excel xls = ExcelHelper.LoadExcel(path);
                if (xls.Tables.Count == 0)
                {
                    ShowTip($"表格文件没有表 {path}.", Color.red, false);
                    Debug.LogError("表格文件没有表");
                    return;
                }

                int rowCount = xls.Tables[0].RowCount;
                if (rowCount <= 1)
                {
                    ShowTip($"表格文件没有数据 {path}.", Color.red, false);
                    Debug.LogError("表格文件没有数据");
                    return;
                }
                List<ItemSaveData> itemDates = new();

                // 获取工作表
                foreach (ExcelTable table in xls.Tables)
                {
                    //获取表格的列和行
                    for (int row = 0; row <= table.RowCount; row++)
                    {
                        // 跳过第一行
                        if (row <= 1) continue;
                        ItemSaveData save = new()
                        {
                            id = int.Parse(table.GetValue(row, 1).ToString()),
                            display_name = table.GetValue(row, 2).ToString(),
                            description = table.GetValue(row, 3).ToString(),
                            itemType = ItemType.None,
                            maxStackSize = int.Parse(table.GetValue(row, 5).ToString()),
                            value = int.Parse(table.GetValue(row, 6).ToString()),
                            iconPath = table.GetValue(row, 7).ToString(),
                            prefabPath = table.GetValue(row, 8).ToString()
                        };
                        foreach (ItemType type in Enum.GetValues(typeof(ItemType)))
                        {
                            if (table.GetValue(row, 4).ToString() == type.ToString())
                                save.itemType = type;
                        }
                        itemDates.Add(save);
                    }
                }

                CreateAssetFromSaveData(ref itemDates, path);
            }
            catch (System.Exception exception)
            {
                ShowTip($"Failed to read data from {path}. \n{exception}", Color.red, false);
                Debug.LogError($"Failed to read data from {path}. \n{exception}");
            }
        }

        /// <summary>
        /// 导出Excel文件
        /// </summary>
        private void OutExeclFile()
        {
            if (!CheckDatabase()) return;
            string path = $"{setting.execlPath}/{database.name}.xlsx";
            try
            {
                // 将所有 itemdata 转换为可序列化的 savedata
                List<ItemSaveData> saveDates = new();
                foreach (ItemData i in database.Items)
                    saveDates.Add(new ItemSaveData(i));

                // 检查文件是否存在，存在则读取，否则创建新文件
                Excel xls = File.Exists(path) ? ExcelHelper.LoadExcel(path) : ExcelHelper.CreateExcel(path);
                ExcelTable table = xls.Tables[0];
                for (int i = 0; i < saveDates.Count; i++)
                {
                    int row = i + 2;
                    table.SetValue(row, 1, saveDates[i].id.ToString());
                    table.SetValue(row, 2, saveDates[i].display_name);
                    table.SetValue(row, 3, saveDates[i].description);
                    table.SetValue(row, 4, saveDates[i].itemType.ToString());
                    table.SetValue(row, 5, saveDates[i].maxStackSize.ToString());
                    table.SetValue(row, 6, saveDates[i].value.ToString());
                    table.SetValue(row, 7, saveDates[i].iconPath);
                    table.SetValue(row, 8, saveDates[i].prefabPath);
                }

                ExcelHelper.SaveExcel(xls, path);
                ShowTip($"Save Execl file to {path}.", Color.yellow);
            }
            catch (System.Exception exception)
            {
                ShowTip($"Failed to save data to {path}. \n{exception}", Color.red, false);
                Debug.LogError($"Failed to save data to {path}. \n{exception}");
            }
        }

        #endregion

        private void CreateAssetFromSaveData(ref List<ItemSaveData> itemDates, string path)
        {
            // 在内存创建database实例
            ItemDatabase database = ScriptableObject.CreateInstance<ItemDatabase>();

            // 向磁盘写入database,在嵌套scriptableobject时必须先写入磁盘进行持久化
            string fileName = Path.GetFileNameWithoutExtension(path);
            // createAsset 需要使用相对路径
            AssetDatabase.CreateAsset(database, $"{setting.newDatabasePath}/{fileName}.asset");

            // 读取全部item数据
            foreach (var data in itemDates)
            {
                ItemData item = ScriptableObject.CreateInstance<ItemData>();
                item.ReadSaveDate(data);
                database.AddItem(item);
            }

            // 保存创建的资源
            EditorUtility.SetDirty(database);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            ShowTip($"Creat ItemDatabase which contain {database.Items.Count} items, in {setting.newDatabasePath}{fileName}.asset", Color.yellow);
            ChangeDatabase(database);
        }

        private void ShowTip(string text, Color color, bool debugLog = true)
        {
            tipLabel.style.color = color;
            tipLabel.text = text;
            if (debugLog) Debug.Log(text);
        }

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
    public class SerializableList<T>
    {
        [SerializeField] private List<T> target;
        public List<T> ToList() { return target; }

        public SerializableList(List<T> target)
        {
            this.target = target;
        }

        // // List<T> -> Json文字列 ( 例 : List<Enemy> )
        // string str = JsonUtility.ToJson(new Serialization<Enemy>(enemies)); 
        // // Json文字列 -> List<T>
        // List<Enemy> enemies = JsonUtility.FromJson<Serialization<Enemy>>(str).ToList();
        // // Dictionary<TKey,TValue> -> Json文字列 ( 例 : Dictionary<int, Enemy> )
        // string str = JsonUtility.ToJson(new Serialization<int, Enemy>(enemies)); 
    }
}
