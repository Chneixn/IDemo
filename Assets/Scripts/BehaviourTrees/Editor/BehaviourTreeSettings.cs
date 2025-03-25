using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace BehaviourTreeSystem
{
    [CreateAssetMenu(menuName = "BehaviourTree/Create BehaviourTreeSetting", fileName = "BehaviourTreeSettings")]
    public class BehaviourTreeSettings : ScriptableObject
    {
        public BehaviourTreeSettings UsingSetting;
        public VisualTreeAsset behaviourTreeUxml;
        public StyleSheet behaviourTreeStyle;
        public VisualTreeAsset nodeUxml;

        public static BehaviourTreeSettings GetOrCreateSettings()
        {
            var settings = FindSettings();
            if (settings == null)
            {
                settings = CreateInstance<BehaviourTreeSettings>();
                AssetDatabase.CreateAsset(settings, "Assets/");
                AssetDatabase.SaveAssets();
            }
            settings.UsingSetting = settings;
            return settings;
        }

        private static BehaviourTreeSettings FindSettings()
        {
            string[] guids = AssetDatabase.FindAssets("t:BehaviourTreeSettings");
            if (guids.Length == 0) return null;
            if (guids.Length > 1)
                Debug.LogWarning($"Found multiple settings files, using the first.");

            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            return AssetDatabase.LoadAssetAtPath<BehaviourTreeSettings>(path);
        }

        public static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(GetOrCreateSettings());
        }
    }

    public static class MyCustomSettingsUIElementsRegister
    {
        [SettingsProvider]
        public static SettingsProvider CreateMyCustomSettingsProvider()
        {
            // First parameter is the path in the Settings window.
            // Second parameter is the scope of this setting: it only appears in the Settings window for the Project scope.
            var provider = new SettingsProvider("Project/MyCustomUIElementsSettings", SettingsScope.Project)
            {
                label = "BehaviourTree",
                // activateHandler is called when the user clicks on the Settings item in the Settings window.
                activateHandler = (searchContext, rootElement) =>
                {
                    var settings = BehaviourTreeSettings.GetSerializedSettings();

                    // rootElement is a VisualElement. If you add any children to it, the OnGUI function
                    // isn't called because the SettingsProvider uses the UIElements drawing framework.
                    var title = new Label()
                    {
                        text = "Behaviour Tree Settings"
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
