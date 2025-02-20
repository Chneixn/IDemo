using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace InventorySystem
{
    [CreateAssetMenu(menuName = "Inventory System/Create InventorySystemEditorSetting", fileName = "InventorySystemEditorSetting")]
    public class InventorySystemEditorSetting : ScriptableObject
    {
        public InventorySystemEditorSetting usingSetting;
        public VisualTreeAsset editorUxml;
        public StyleSheet editorStyle;
        public VisualTreeAsset createEditorUxml;
        public StyleSheet createEditorStyle;
        public VisualTreeAsset itemIDEditorUxml;
        public StyleSheet itemIDEditorStyle;
        public string newDatabasePath = "/InventorySystem";
        public string execlPath = "/InventorySystem";

        public static InventorySystemEditorSetting GetOrCreateSettings()
        {
            // 获取设置文件，若无则立即创建一个
            var setting = FindSetting();
            if (setting == null)
            {
                setting = CreateInstance<InventorySystemEditorSetting>();
                AssetDatabase.CreateAsset(setting, "Assets/");
                AssetDatabase.SaveAssets();
            }
            setting.usingSetting = setting;
            return setting;
        }

        private static InventorySystemEditorSetting FindSetting()
        {
            // 全局查找设置文件，当文件多于一个时使用第一个
            string[] guids = AssetDatabase.FindAssets("t:InventorySystemEditorSetting");
            if (guids.Length == 0) return null;
            else if (guids.Length > 1)
                Debug.LogWarning($"Found multiple settings files, using the first.");

            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            return AssetDatabase.LoadAssetAtPath<InventorySystemEditorSetting>(path);
        }

        public static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(GetOrCreateSettings());
        }
    }

    public static class MyCustomSettingsUIElementsRegister
    {
        // 在ProjectSetting创建设置面板
        [SettingsProvider]
        public static SettingsProvider CreateMyCustomSettingsProvider()
        {
            // First parameter is the path in the Settings window.
            // 第一个参数是设置窗口中的路径。
            // Second parameter is the scope of this setting: it only appears in the Settings window for the Project scope.
            // 第二个参数是此设置的作用域：它仅出现在项目范围内的设置窗口中。
            var provider = new SettingsProvider("Project/MyCustomUIElementsSettings", SettingsScope.Project)
            {
                label = "InventorySystem",
                // activateHandler is called when the user clicks on the Settings item in the Settings window.
                // 当用户点击设置窗口中的设置项时，调用activateHandler。
                activateHandler = (searchContext, rootElement) =>
                {
                    var settings = InventorySystemEditorSetting.GetSerializedSettings();

                    // rootElement is a VisualElement. If you add any children to it, the OnGUI function
                    // isn't called because the SettingsProvider uses the UIElements drawing framework.
                    // rootElement 是一个 VisualElement。如果你向其中添加任何子元素，则不会调用 OnGUI 函数，
                    // 因为 SettingsProvider 使用 UIElements 绘图框架。
                    var title = new Label()
                    {
                        text = "InventorySystem Editor Settings"
                    };
                    title.AddToClassList("title");
                    rootElement.Add(title);

                    var properties = new VisualElement()
                    {
                        style =
                    {
                        flexDirection = FlexDirection.Column
                    }
                    };
                    properties.AddToClassList("property-list");
                    rootElement.Add(properties);

                    properties.Add(new InspectorElement(settings));

                    rootElement.Bind(settings);
                },
            };

            return provider;
        }
    }
}