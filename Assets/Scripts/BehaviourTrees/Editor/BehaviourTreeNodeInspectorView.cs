using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor;
using BehaviourTreeSystem;
using UnityEditor.UIElements;

//This not a bug. Just Unity didn't allow.
//namespace BehaviourTreesSystem
//{
public class BehaviourTreeNodeInspectorView : ScrollView
{
    public new class UxmlFactory : UxmlFactory<BehaviourTreeNodeInspectorView, VisualElement.UxmlTraits> { }

    public BehaviourTreeNodeInspectorView() { }

    /// <summary>
    /// 将视图中的Inspector窗口与选择资源时显示的Inspector同步
    /// </summary>
    /// <param name="nodeView"></param>
    public void UpdateSelection(NodeView nodeView)
    {
        Clear();
        Add(new InspectorElement(new SerializedObject(nodeView.node)));
    }
}
//}

