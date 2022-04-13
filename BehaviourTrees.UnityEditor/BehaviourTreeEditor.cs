using BehaviourTrees.UnityEditor.UIElements;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviourTrees.UnityEditor
{
    public class BehaviourTreeEditor : EditorWindow
    {
        private VisualElement _curtain;
        private SplitView _splitView;
        private static BehaviourTreeEditor _instance;

        public BlackboardView Blackboard { get; private set; }
        public InspectorView Inspector { get; private set; }
        public BehaviourTreeView TreeView { get; private set; }
        [CanBeNull] public EditorTreeContainer TreeContainer { get; private set; }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object 
            var root = rootVisualElement;

            // Import UXML
            var visualTree = TreeEditorUtility.GetVisualTree(nameof(BehaviourTreeEditor));
            visualTree.CloneTree(root);

            // A stylesheet can be added to a VisualElement.
            // The style will be applied to the VisualElement and all of its children.
            var styleSheet = TreeEditorUtility.GetStyleSheet();
            root.styleSheets.Add(styleSheet);

            root.schedule.Execute(() => root.styleSheets.Add(styleSheet));

            _splitView = root.Q<SplitView>();
            _curtain = root.Q("graph-curtain");
            TreeView = root.Q<BehaviourTreeView>();
            Inspector = root.Q<InspectorView>();
            Blackboard = root.Q<BlackboardView>();

            root.Q<ToolbarMenu>("toolbar-view").menu
                .AppendAction("Problems Window", _ => ProblemsWindow.OpenWindow());

            root.Q<ToolbarButton>("toolbar-format").clicked += () => TreeFormatter.FormatTreeStructure(TreeView);

            TreeView.TreeLoaded += RemoveCurtain;
            TreeView.SelectionChanged += Inspector.SetToNode;
        }

        public static BehaviourTreeEditor GetOrOpen()
        {
            return _instance != null ? _instance : CreateWindow();
        }

        private void LoadTree(EditorTreeContainer container)
        {
            TreeContainer = container;
            TreeView.PopulateView();
            Blackboard.UpdateBlackboard();
        }

        [MenuItem("Window/AI/Behaviour Tree Editor")]
        public static BehaviourTreeEditor CreateWindow()
        {
            var wnd = GetWindow<BehaviourTreeEditor>("Behaviour Tree", true, typeof(SceneView));
            wnd.titleContent = new GUIContent("Behaviour Tree", TreeEditorUtility.GetEditorIcon());
            _instance = wnd;
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
                var editor = GetOrOpen();
                editor.LoadTree(treeContainer);
            }

            return false;
        }
    }
}