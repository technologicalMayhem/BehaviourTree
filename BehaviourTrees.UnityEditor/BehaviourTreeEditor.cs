using System;
using BehaviourTrees.Model;
using BehaviourTrees.UnityEditor.UIElements;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviourTrees.UnityEditor
{
    public class BehaviourTreeEditor : EditorWindow
    {
        private VisualElement _curtain;
        private InspectorView _inspector;
        private BlackboardView _blackboard;
        private SplitView _splitView;
        private BehaviourTreeView _treeView;

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
            _treeView.LoadTree(treeContainer);
        }

        private static BehaviourTreeEditor CreateWindow()
        {
            var wnd = GetWindow<BehaviourTreeEditor>("Behaviour Tree Editor", true, typeof(SceneView));
            wnd.titleContent = new GUIContent("Behaviour Tree Editor");
            return wnd;
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object 
            var root = rootVisualElement;

            // Import UXML
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                EditorUtilities.LocateUiDefinitionFile(nameof(BehaviourTreeEditor)));
            visualTree.CloneTree(root);

            // A stylesheet can be added to a VisualElement.
            // The style will be applied to the VisualElement and all of its children.
            var styleSheet = EditorUtilities.GetStyleSheet();
            root.styleSheets.Add(styleSheet);

            root.schedule.Execute(() => root.styleSheets.Add(styleSheet));

            _splitView = root.Q<SplitView>();
            _curtain = root.Q("graph-curtain");
            _treeView = root.Q<BehaviourTreeView>();
            _inspector = root.Q<InspectorView>();
            _blackboard = root.Q<BlackboardView>();
            _inspector.TreeView = _treeView;
            _blackboard.TreeView = _treeView;

            _treeView.TreeLoaded += () =>
            {
                _curtain.style.display = new StyleEnum<DisplayStyle>(StyleKeyword.None);
                _splitView.style.display = new StyleEnum<DisplayStyle>(StyleKeyword.Auto);
            };
            _treeView.SelectionChanged += nodeView => { _inspector.SetToNode(nodeView); };
        }

        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceId, int line)
        {
            var assetPath = AssetDatabase.GetAssetPath(instanceId);
            var treeContainer = AssetDatabase.LoadAssetAtPath<EditorTreeContainer>(assetPath);
            if (treeContainer != null)
            {
                treeContainer.TreeModel ??= new ConceptualBehaviourTree();
                OpenWindow(treeContainer);
            }

            return false;
        }
    }
}