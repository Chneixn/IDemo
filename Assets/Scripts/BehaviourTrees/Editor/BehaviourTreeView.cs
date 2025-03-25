#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using BehaviourTreeSystem;

public class BehaviourTreeView : GraphView
{
    public Action<NodeView> OnNodeSelected;
    public new class UxmlFactory : UxmlFactory<BehaviourTreeView, UxmlTraits> { }
    private BehaviourTree tree;
    BehaviourTreeSettings settings;
    private Vector2 clickPosition;

    public BehaviourTreeView()
    {
        settings = BehaviourTreeSettings.GetOrCreateSettings();
        Insert(0, new GridBackground());

        this.AddManipulator(new ContentZoomer());       //允许缩放
        this.AddManipulator(new ContentDragger());      //允许鼠标拖动多个节点
        this.AddManipulator(new SelectionDragger());    //允许拖动节点
        this.AddManipulator(new RectangleSelector());   //矩形选择器(框选多个节点功能)

        var styleSheet = settings.behaviourTreeStyle;
        styleSheets.Add(styleSheet);

        Undo.undoRedoPerformed += OnUndoRedo;
    }

    private void OnUndoRedo()
    {
        if (tree != null)
        {
            PopulateView(tree);
            AssetDatabase.SaveAssets();
        }
    }

    private NodeView FindNodeView(BehaviourTreeSystem.Node node)
    {
        return GetNodeByGuid(node.guid) as NodeView;
    }

    /// <summary>
    /// 刷新视图
    /// </summary>
    /// <param name="tree"></param>
    public void PopulateView(BehaviourTree tree)
    {
        graphViewChanged -= OnGraphViewChanged;
        DeleteElements(graphElements);
        graphViewChanged += OnGraphViewChanged;

        if (tree == null) return;
        this.tree = tree;

        if (tree.rootNode == null)
        {
            tree.rootNode = tree.CreateNode(typeof(RootNode)) as RootNode;
            EditorUtility.SetDirty(tree);
            AssetDatabase.SaveAssets();
        }

        // 创建节点视图
        tree.nodes.ForEach(n => CreateNodeView(n));

        // 创建连接线
        tree.nodes.ForEach(n =>
        {
            var children = tree.GetChildren(n);
            children.ForEach(c =>
            {
                NodeView parentView = FindNodeView(n);
                NodeView childView = FindNodeView(c);

                Edge edge = parentView.output.ConnectTo(childView.input);
                AddElement(edge);
            });
        });
    }

    public void ClearView()
    {
        graphViewChanged -= OnGraphViewChanged;
        DeleteElements(graphElements);
        graphViewChanged += OnGraphViewChanged;
    }

    /// <summary>
    /// 创建节点间的连线
    /// </summary>
    /// <param name="startPort"></param>
    /// <param name="nodeAdapter"></param>
    /// <returns></returns>
    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        return ports.ToList().Where(endPort =>
        endPort.direction != startPort.direction &&
        endPort.node != startPort.node).ToList();
    }

    /// <summary>
    /// 当视图发生任何改变时
    /// </summary>
    /// <param name="graphViewChange"></param>
    /// <returns></returns>
    private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
    {
        //在视图中删除元素时
        if (graphViewChange.elementsToRemove != null)
        {
            graphViewChange.elementsToRemove.ForEach(elem =>
            {
                //当节点被删除时
                if (elem is NodeView nodeView)
                    tree.DeleteNode(nodeView.node);

                //当连线被删除时
                if (elem is Edge edge)
                {
                    NodeView parentView = edge.output.node as NodeView;
                    NodeView childView = edge.input.node as NodeView;
                    tree.RemoveChild(parentView.node, childView.node);
                }
            });
        }

        //当有新的连接线创建时
        if (graphViewChange.edgesToCreate != null)
        {
            graphViewChange.edgesToCreate.ForEach(edge =>
            {
                NodeView parentView = edge.output.node as NodeView;
                NodeView childView = edge.input.node as NodeView;
                tree.AddChild(parentView.node, childView.node);
            });
        }

        //当节点在视图中移动时
        if (graphViewChange.movedElements != null)
        {
            nodes.ForEach((n) =>
            {
                NodeView view = n as NodeView;
                view.SortChildren();
                view.Save();
            });
        }

        return graphViewChange;
    }

    /// <summary>
    /// 在右键菜单添加选项
    /// Add menu items to the contextual menu. API might changed or removed in the future.
    /// </summary>
    /// <param name="evt"></param>
    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        base.BuildContextualMenu(evt);
        // 获取在视图中的鼠标坐标
        clickPosition = contentViewContainer.WorldToLocal(evt.mousePosition);

        {
            //获取所有继承于ActionNode的类并显示在右键菜单中
            var types = TypeCache.GetTypesDerivedFrom<ActionNode>();
            foreach (var type in types)
            {
                evt.menu.AppendAction($"Action/ {type.Name}", (a) => CreateNode(type));
            }
        }

        {
            // 获取所有继承于ActionNode的类并显示在右键菜单中
            var types = TypeCache.GetTypesDerivedFrom<ConditionalNode>();
            foreach (var type in types)
            {
                evt.menu.AppendAction($"Condition/ {type.Name}", (a) => CreateNode(type));
            }
        }

        {
            var types = TypeCache.GetTypesDerivedFrom<CompositeNode>();
            foreach (var type in types)
            {
                evt.menu.AppendAction($"Composite/ {type.Name}", (a) => CreateNode(type));
            }
        }

        {
            var types = TypeCache.GetTypesDerivedFrom<DecoratorNode>();
            foreach (var type in types)
            {
                evt.menu.AppendAction($"Decorator/ {type.Name}", (a) => CreateNode(type));
            }
        }
    }

    private void CreateNode(System.Type type)
    {
        if (!tree)
        {
            Debug.LogWarning("You have to select an BehaviourTree assest first!");
            return;
        }
        BehaviourTreeSystem.Node node = tree.CreateNode(type);
        CreateNewNodeView(node);
    }

    private void CreateNewNodeView(BehaviourTreeSystem.Node node)
    {
        NodeView nodeView = new(node);
        nodeView.OnNodeSelected = OnNodeSelected;
        nodeView.SetPosition(new Rect(clickPosition, nodeView.GetPosition().size));
        AddElement(nodeView);
    }

    private void CreateNodeView(BehaviourTreeSystem.Node node)
    {
        NodeView nodeView = new(node);
        nodeView.OnNodeSelected = OnNodeSelected;
        //nodeView.SetPosition(new Rect(nodeView.node.position, nodeView.GetPosition().size));
        AddElement(nodeView);
    }

    public void OnSave()
    {
        nodes.ForEach(n =>
        {
            if (n is NodeView view)
                view.Save();
        });
    }

    public void UpdateNodeStates()
    {
        nodes.ForEach(n =>
        {
            if (n is NodeView view)
                view.UpdateState();
        });
    }
}

#endif
