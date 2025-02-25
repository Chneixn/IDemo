using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BaseGun))]
public class GunSettingShowOnInspector : Editor
{
    public override void OnInspectorGUI()
    {
        // Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
        serializedObject.Update();
        base.OnInspectorGUI();

        GunSetting set = ((BaseGun)target).set;
        if (set != null)
        {
            var settingEditor = Editor.CreateEditor(set);
            settingEditor.OnInspectorGUI();
        }
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
