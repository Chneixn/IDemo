using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using InventorySystem;

// TODO:按下apply按键后不保存修改
public class ItemCreateEditor : EditorWindow
{
    public InventorySystemEditor editor;
    public ItemDatabase database;
    private ItemData itemData;
    private InventoryItemInspectorView inspectorView;
    private Toggle autoID;
    private Button applyButton;
    private Button canelButton;
    private bool isApply = false;

    public static ItemCreateEditor OpenWindow()
    {
        // true 属性使该窗口不可停靠
        ItemCreateEditor window = GetWindow<ItemCreateEditor>(true, "ItemCreateEditor");
        window.minSize = new Vector2(250, 450);
        //手动聚焦
        window.Focus();
        return window;
    }

    public void CreateGUI()
    {
        var setting = InventorySystemEditorSetting.GetOrCreateSettings();

        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // 获取 UXML
        VisualTreeAsset visualTree = setting.createEditorUxml;
        visualTree.CloneTree(root);

        StyleSheet styleSheet = setting.createEditorStyle;
        root.styleSheets.Add(styleSheet);

        inspectorView = root.Q<InventoryItemInspectorView>("inspector-view");
        inspectorView.Init(root.Q<VisualElement>("icon-image"));

        autoID = root.Q<Toggle>("auto-id");

        applyButton = root.Q<Button>("apply-button");
        applyButton.clicked += OnApply;

        canelButton = root.Q<Button>("canel-button");
        canelButton.clicked += OnCanel;
    }

    public void Init(InventorySystemEditor editor, ItemDatabase database)
    {
        this.editor = editor;
        this.database = database;
        itemData = database.CreateItem();
        inspectorView.UpdateSelection(itemData); ;
    }

    private void OnInspectorUpdate()
    {
        inspectorView.UpdateImage();
    }

    public void OnApply()
    {
        isApply = true;

        if (autoID.value == true) database.SetItemIDs();

        itemData.name = $"[{itemData.ID}]{itemData.displayName}";

        EditorUtility.SetDirty(database);
        AssetDatabase.SaveAssets();
        editor.OnRefresh();

        Close();
    }

    public void OnCanel()
    {
        Close();
    }

    private void OnDestroy()
    {
        if (isApply == false)
            database.DeleteItem(itemData);
    }
}
