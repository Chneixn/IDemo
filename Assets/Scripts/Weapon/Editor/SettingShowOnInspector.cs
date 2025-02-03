using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BaseGun))]
public class SettingShowOnInspector : Editor
{
    Editor settingEditor;

    public override void OnInspectorGUI()
    {
        // Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
        serializedObject.Update();
        base.OnInspectorGUI();
        GUILayout.Space(8);

        GunSetting set = ((BaseGun)target).set;
        if (set != null)
        {
            if (settingEditor == null)
            {
                settingEditor = Editor.CreateEditor(set);
            }
            settingEditor.OnInspectorGUI();
        }
    }
}

// [CustomPropertyDrawer(typeof(GunSetting))]
// public class ShowClassOnInspectorDrawer : PropertyDrawer
// {
//     public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//     {
//         var settingEditor = Editor.CreateEditor(property.boxedValue as GunSetting);
//         if (settingEditor != null)
//         {
//             settingEditor.OnInspectorGUI();
//         }

//     }
// }
