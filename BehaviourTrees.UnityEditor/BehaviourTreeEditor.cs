using BehaviourTrees.Model;
using BehaviourTrees.UnityEditor.UIElements;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviourTrees.UnityEditor
{
    public class BehaviourTreeEditor : EditorWindow
    {
        private BlackboardView _blackboard;
        private VisualElement _curtain;
        private InspectorView _inspector;
        private SplitView _splitView;
        private BehaviourTreeView _treeView;

        public BehaviourTreeView TreeView => _treeView;

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object 
            var root = rootVisualElement;

            // Import UXML
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                TreeEditorUtility.LocateUiDefinitionFile(nameof(BehaviourTreeEditor)));
            visualTree.CloneTree(root);

            // A stylesheet can be added to a VisualElement.
            // The style will be applied to the VisualElement and all of its children.
            var styleSheet = TreeEditorUtility.GetStyleSheet();
            root.styleSheets.Add(styleSheet);

            root.schedule.Execute(() => root.styleSheets.Add(styleSheet));

            _splitView = root.Q<SplitView>();
            _curtain = root.Q("graph-curtain");
            _treeView = root.Q<BehaviourTreeView>();
            _inspector = root.Q<InspectorView>();
            _blackboard = root.Q<BlackboardView>();
            _inspector.BlackboardView = _blackboard;

            root.Q<ToolbarButton>("show-problems").clicked += ProblemsWindow.OpenWindow;

            _treeView.TreeLoaded += RemoveCurtain;
            _treeView.SelectionChanged += _inspector.SetToNode;
        }

        [MenuItem("Window/AI/Behaviour Tree Editor")]
        public static void OpenWindow()
        {
            CreateWindow();
        }

        private static void OpenWindow(EditorTreeContainer treeContainer)
        {
            var window = CreateWindow();
            window.LoadTree(treeContainer);
        }

        private void LoadTree(EditorTreeContainer treeContainer)
        {
            _inspector.Tree = treeContainer;
            _blackboard.Container = treeContainer;
            _treeView.LoadTree(treeContainer);
        }

        private static BehaviourTreeEditor CreateWindow()
        {
            var wnd = GetWindow<BehaviourTreeEditor>("Behaviour Tree", true, typeof(SceneView));
            wnd.titleContent = new GUIContent("Behaviour Tree", TreeEditorUtility.GetEditorIcon());
            return wnd;
        }

        private void RemoveCurtain()
        {
            _curtain.AddToClassList("hide");
            _splitView.RemoveFromClassList("hide");
        }

        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceId, int line)
        {
            var assetPath = AssetDatabase.GetAssetPath(instanceId);
            var treeContainer = AssetDatabase.LoadAssetAtPath<EditorTreeContainer>(assetPath);
            if (treeContainer != null)
            {
                treeContainer.TreeModel ??= new BehaviourTreeModel();
                OpenWindow(treeContainer);
            }

            return false;
        }
    }
}