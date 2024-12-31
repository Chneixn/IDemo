using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using BehaviourTreesSystem;

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
        blackboardProperty = treeObject.FindProperty(tree.blackboard.ToString().ToLower());
        // Debug.Log(tree.blackboard.ToString().ToLower());

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
