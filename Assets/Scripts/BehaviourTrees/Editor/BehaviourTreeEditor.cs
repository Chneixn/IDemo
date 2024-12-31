using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Callbacks;
using UnityEditor.UIElements;
using System;

namespace BehaviourTreesSystem
{
    public class BehaviourTreeEditor : EditorWindow
    {
        // View panels
        BehaviourTreeView treeView;
        BehaviourTreeNodeInspectorView inspectorView;
        BlackboardView blackboardView;

        // Button and title
        ToolbarMenu assetMenu;
        ToolbarButton saveButton;
        Label editorTitle;

        BehaviourTreeSettings setting;

        /// <summary>
        /// 打开BehaviourTreeEditor视图
        /// </summary>
        [MenuItem("CustomSystem/BehaviourTreeEditor")]
        public static void OpenWindow()
        {
            BehaviourTreeEditor window = GetWindow<BehaviourTreeEditor>();
            window.titleContent = new GUIContent("BehaviourTreeEditor");
            window.minSize = new Vector2(800, 600);
        }

        /// <summary>
        /// 实现双击资源文件时打开可视化编辑视图
        /// </summary>
        /// <param name="instanceId">资源的id</param>
        /// <param name="line"></param>
        /// <returns></returns>
        [OnOpenAsset]
        public static bool OnPoenAsset(int instanceId, int line)
        {
            if (Selection.activeObject is BehaviourTree)
            {
                OpenWindow();
                return true;
            }
            return false;
        }

        public void CreateGUI()
        {
            setting = BehaviourTreeSettings.GetOrCreateSettings();

            // 根元素
            VisualElement root = rootVisualElement;

            // Import UXML
            VisualTreeAsset visualTree = setting.behaviourTreeUxml;
            visualTree.CloneTree(root);

            // stylesheet 可以 Add 到 VisualElement.
            // 这个 style 会应用到所有子元素
            StyleSheet styleSheet = setting.behaviourTreeStyle;
            root.styleSheets.Add(styleSheet);

            // 从 root 中以 .Q<T>(name) 查找元素
            treeView = root.Q<BehaviourTreeView>("TreeView");
            treeView.OnNodeSelected = OnNodeSelectionChange;

            inspectorView = root.Q<BehaviourTreeNodeInspectorView>("InsectorView");

            blackboardView = root.Q<BlackboardView>("BlackboardView");

            // 获取 save 按钮，绑定按钮响应
            saveButton = root.Q<ToolbarButton>("SaveButton");
            saveButton.clicked += OnSave;
            saveButton.clicked += treeView.OnSave;

            // 获取 assetmenu 按钮，绑定按钮响应
            assetMenu = root.Q<ToolbarMenu>("AssetMenu");
            UpdateAssetMenu();

            // 获取 editortitle 组件，用于显示当前行为树的名称
            editorTitle = root.Q<Label>("EditorTitle");

            OnSelectionChange();
        }

        private void OnEnable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChange;
            EditorApplication.playModeStateChanged += OnPlayModeStateChange;

            EditorApplication.projectChanged += OnProjectChanged;
        }

        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChange;
            EditorApplication.projectChanged -= OnProjectChanged;
        }

        /// <summary>
        /// 响应当项目文件出现变化时（增删文件，文件重命名等）
        /// </summary>
        private void OnProjectChanged()
        {
            UpdateAssetMenu();
        }

        /// <summary>
        /// 刷新AssetMenu的选项菜单
        /// </summary>
        private void UpdateAssetMenu()
        {
            if (assetMenu == null) return;

            //清空选项
            if (assetMenu.menu.MenuItems().Count != 0)
            {
                assetMenu.menu.ClearItems();

                // for (int i = assetMenu.menu.MenuItems().Count - 1; i > 0; i--)
                // {
                //     assetMenu.menu.RemoveItemAt(i);
                // }
            }

            //获取项目文件中所有行为树的guid
            string[] guids = AssetDatabase.FindAssets("t:BehaviourTree");
            foreach (string guid in guids)
            {
                //获取树类
                string path = AssetDatabase.GUIDToAssetPath(guid);
                BehaviourTree tree = AssetDatabase.LoadAssetAtPath<BehaviourTree>(path);

                // 给选项添加响应，刷新当前选择对象为该行为树
                assetMenu.menu.AppendAction($"{tree.name}", (t) =>
                {
                    Selection.activeObject = tree;
                });
            }
        }

        private void OnPlayModeStateChange(PlayModeStateChange stateChange)
        {
            switch (stateChange)
            {
                case PlayModeStateChange.EnteredEditMode:
                    OnSelectionChange();
                    break;
                case PlayModeStateChange.ExitingEditMode:
                    break;
                case PlayModeStateChange.EnteredPlayMode:
                    OnSelectionChange();
                    break;
                case PlayModeStateChange.ExitingPlayMode:
                    break;
            }
        }

        /// <summary>
        /// 重新获取行为树资源文件，若获取到则刷新行为树界面
        /// </summary>
        private void OnSelectionChange()
        {
            EditorApplication.delayCall += () =>
            {
                //获取选择对象
                BehaviourTree tree = Selection.activeObject as BehaviourTree;
                if (!tree)
                {
                    //实现在选中挂载BehaviourTreeRunner的Gameobject时，切换可视化视图中的BehaviourTree
                    if (Selection.activeGameObject)
                    {
                        BehaviourTreeRunner runner = Selection.activeGameObject.GetComponent<BehaviourTreeRunner>();
                        if (runner)
                        {
                            tree = runner.tree;
                        }
                    }
                }

                if (tree)
                    UpdateTreeView(tree);
            };
        }

        /// <summary>
        /// 刷新行为树界面
        /// </summary>
        /// <param name="tree"></param>
        private void UpdateTreeView(BehaviourTree tree)
        {
            //当选择是BehaviourTree时则刷新视图
            if (Application.isPlaying)
            {
                //在PlayMode选择时刷新
                if (tree)
                {
                    treeView.PopulateView(tree);
                }
            }
            else if (tree && AssetDatabase.CanOpenAssetInEditor(tree.GetInstanceID()))
            {
                //在资源文件中选择刷新
                treeView.PopulateView(tree);
            }

            if (tree)
            {
                //blackboard页面刷新
                blackboardView.UpdateSelection(tree);

                //修改Editor标题
                editorTitle.text = tree.name;
            }
        }

        private void OnNodeSelectionChange(NodeView node)
        {
            inspectorView.UpdateSelection(node);
        }

        private void OnSave()
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void OnInspectorUpdate()
        {
            treeView?.UpdateNodeStates();
        }

        private void OnDestroy()
        {
            AssetDatabase.SaveAssets();
        }
    }
}

