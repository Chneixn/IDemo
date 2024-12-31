using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor;
using BehaviourTreesSystem;

//This not a bug. Just Unity didn't allow.
//namespace BehaviourTreesSystem
//{
public class BehaviourTreeNodeInspectorView : ScrollView
{
    public new class UxmlFactory : UxmlFactory<BehaviourTreeNodeInspectorView, VisualElement.UxmlTraits> { }

    Editor editor;

    public BehaviourTreeNodeInspectorView() { }

    /// <summary>
    /// 将视图中的Inspector窗口与选择资源时显示的Inspector同步
    /// </summary>
    /// <param name="nodeView"></param>
    public void UpdateSelection(NodeView nodeView)
    {
        Clear();

        UnityEngine.Object.DestroyImmediate(editor);
        editor = Editor.CreateEditor(nodeView.node);

        IMGUIContainer container = new(() =>
        {
            if (editor && editor.target)
            {
                editor.OnInspectorGUI();
            }
        });
        Add(container);
    }
}
//}

