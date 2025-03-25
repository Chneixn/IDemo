using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using BehaviourTreeSystem;

public class BlackboardView : ScrollView
{
    public new class UxmlFactory : UxmlFactory<BlackboardView, VisualElement.UxmlTraits> { }

    public BlackboardView() { }

    SerializedObject treeObject;
    SerializedProperty blackboardProperty;

    /// <summary>
    /// 将视图中的Blackboard窗口与当前BehaviorTree的Blackboard同步
    /// </summary>
    /// <param name="tree"></param>
    public void UpdateSelection(BehaviourTree tree)
    {
        Clear();

        // 刷新treeObject缓存为当前选择tree
        treeObject = new SerializedObject(tree);
        string blackboardName = tree.blackboard.ToString();
        int dotIndex = blackboardName.IndexOf('.');
        if (dotIndex != -1) blackboardName = blackboardName[(dotIndex + 1)..];

        blackboardProperty = treeObject.FindProperty(blackboardName.ToLower());

        IMGUIContainer container = new(() =>
        {
            if (treeObject != null && treeObject.targetObject != null)
            {
                treeObject.Update();
                EditorGUILayout.PropertyField(blackboardProperty);
                treeObject.ApplyModifiedProperties();
            }
        });

        Add(container);
    }
}
