using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BehaviourTreeSystem
{
    [CreateAssetMenu(menuName = "BehaviourTree/Create new BehaviourTree", fileName = "New BehaviourTree")]
    public class BehaviourTree : ScriptableObject
    {
        public bool isLog = false;
        public Node rootNode;
        public Node.State treeState = Node.State.Running;
        public List<Node> nodes = new();
        public Blackboard blackboard = new();

        public Node.State Update()
        {
            if (rootNode.state == Node.State.Running)
            {
                treeState = rootNode.Update();
            }
            return treeState;
        }

#if UNITY_EDITOR
        public Node CreateNode(System.Type type)
        {
            Node node = ScriptableObject.CreateInstance(type) as Node;
            node.name = type.Name;
            node.guid = GUID.Generate().ToString();

            Undo.RecordObject(this, "Behaviour Tree (CreateNode)");
            nodes.Add(node);

            if (!Application.isPlaying)
            {
                AssetDatabase.AddObjectToAsset(node, this);
            }

            Undo.RegisterCreatedObjectUndo(node, "Behaviour Tree (CreateNode)");

            AssetDatabase.SaveAssets();
            return node;
        }

        public void DeleteNode(Node node)
        {
            Undo.RecordObject(this, "Behaviour Tree (DeleteNode)");

            nodes.Remove(node);
            //AssetDatabase.RemoveObjectFromAsset(node);
            Undo.DestroyObjectImmediate(node);

            AssetDatabase.SaveAssets();
        }

        public void AddChild(Node parent, Node child)
        {
            RootNode root = parent as RootNode;
            if (root)
            {
                Undo.RecordObject(root, "Behaviour Tree (AddChild)");
                root.child = child;
                EditorUtility.SetDirty(root);
            }

            DecoratorNode decorator = parent as DecoratorNode;
            if (decorator)
            {
                Undo.RecordObject(decorator, "Behaviour Tree (AddChild)");
                decorator.child = child;
                EditorUtility.SetDirty(decorator);
            }

            CompositeNode composite = parent as CompositeNode;
            if (composite)
            {
                Undo.RecordObject(composite, "Behaviour Tree (AddChild)");
                composite.children.Add(child);
                EditorUtility.SetDirty(composite);
            }
        }

        public void RemoveChild(Node parent, Node child)
        {
            RootNode root = parent as RootNode;
            if (root)
            {
                Undo.RecordObject(root, "Behaviour Tree (RemoveChild)");
                root.child = null;
                EditorUtility.SetDirty(root);
            }

            DecoratorNode decorator = parent as DecoratorNode;
            if (decorator)
            {
                Undo.RecordObject(decorator, "Behaviour Tree (RemoveChild)");
                decorator.child = null;
                EditorUtility.SetDirty(decorator);
            }

            CompositeNode composite = parent as CompositeNode;
            if (composite)
            {
                Undo.RecordObject(composite, "Behaviour Tree (RemoveChild)");
                composite.children.Remove(child);
                EditorUtility.SetDirty(composite);
            }
        }

        public List<Node> GetChildren(Node parent)
        {
            List<Node> children = new();

            RootNode root = parent as RootNode;
            if (root && root.child != null)
                children.Add(root.child);

            DecoratorNode decorator = parent as DecoratorNode;
            if (decorator && decorator.child != null)
                children.Add(decorator.child);

            CompositeNode composite = parent as CompositeNode;
            if (composite)
                return composite.children;

            return children;
        }
#endif
        /// <summary>
        /// 遍历行为树的节点
        /// </summary>
        /// <param name="node"></param>
        /// <param name="visiter">对每个访问到的结点执行Action</param>
        public void Traverse(Node node, System.Action<Node> visiter)
        {
            if (node)
            {
                visiter.Invoke(node);
                var children = GetChildren(node);
                children.ForEach((n) => Traverse(n, visiter));
            }
        }

        public BehaviourTree Clone()
        {
            BehaviourTree tree = Instantiate(this);
            tree.rootNode = tree.rootNode.Clone();
            tree.nodes = new List<Node>();
            Traverse(tree.rootNode, (n) =>
            {
                n.blackboard = tree.blackboard;
                n.isLog = tree.isLog;
                tree.nodes.Add(n);
            });
            return tree;
        }

        // public void Bind(Agent agent)
        // {
        //     Traverse(rootNode, node =>
        //     {
        //         node.agent = agent;
        //         node.blackboard = blackboard;
        //     });
        // }
    }
}