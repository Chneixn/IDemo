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
            // 全局查找设置文件，当文件多于一个时警告
            string[] guids = AssetDatabase.FindAssets("t:InventorySystemEditorSetting");
            if (guids.Length == 0) return null;
            if (guids.Length > 1)
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
            // Second parameter is the scope of this setting: it only appears in the Settings window for the Project scope.
            var provider = new SettingsProvider("Project/MyCustomUIElementsSettings", SettingsScope.Project)
            {
                label = "InventorySystem",
                // activateHandler is called when the user clicks on the Settings item in the Settings window.
                activateHandler = (searchContext, rootElement) =>
                {
                    var settings = InventorySystemEditorSetting.GetSerializedSettings();

                    // rootElement is a VisualElement. If you add any children to it, the OnGUI function
                    // isn't called because the SettingsProvider uses the UIElements drawing framework.
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

