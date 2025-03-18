using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(Gun), true)]
public class GunSettingShowOnInspector : Editor
{
    public override VisualElement CreateInspectorGUI()
    {
        var gun = (Gun)target;
        var root = new VisualElement();
        root.Add(new IMGUIContainer(OnInspectorGUI));

        if (gun.Setting != null)
        {
            var title = new Label()
            {
                text = "Gun Settings"
            };
            root.Add(title);
            SerializedObject obj = new(gun.Setting);
            var element = new InspectorElement(obj);
            element.Bind(obj);
            root.Add(element);
        }

        return root;
    }
}

[CustomEditor(typeof(Knife))]
public class KnifeSettingShowOnInspector : Editor
{
    public override void OnInspectorGUI()
    {
        // Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
        serializedObject.Update();
        base.OnInspectorGUI();

        KnifeSetting set = ((Knife)target).setting;
        if (set != null)
        {
            var settingEditor = Editor.CreateEditor(set);
            settingEditor.OnInspectorGUI();
        }
    }
}
