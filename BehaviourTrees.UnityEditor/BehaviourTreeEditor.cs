using BehaviourTrees.UnityEditor.UIElements;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviourTrees.UnityEditor
{
    /// <summary>
    ///     A editor window for behaviour trees.
    /// </summary>
    public class BehaviourTreeEditor : EditorWindow
    {
        /// <summary>
        ///     The curtain element in the editor.
        ///     It gets shown instead of the <see cref="BehaviourTreeView" /> when noting is loaded.
        /// </summary>
        private VisualElement _curtain;

        /// <summary>
        ///     The <see cref="SplitView" /> element in the editor.
        /// </summary>
        private SplitView _splitView;

        /// <summary>
        ///     A reference to the currently opened window.
        /// </summary>
        private static BehaviourTreeEditor _instance;

        /// <summary>
        ///     The <see cref="BlackboardView" /> element in the editor.
        /// </summary>
        public BlackboardView Blackboard { get; private set; }

        /// <summary>
        ///     The <see cref="InspectorView" /> element in the editor.
        /// </summary>
        public InspectorView Inspector { get; private set; }

        /// <summary>
        ///     The <see cref="BehaviourTreeView" /> element in the editor.
        /// </summary>
        public BehaviourTreeView TreeView { get; private set; }

        /// <summary>
        ///     The currently loaded <see cref="EditorTreeContainer" /> instance.
        /// </summary>
        [CanBeNull] public EditorTreeContainer TreeContainer { get; private set; }

        /// <summary>
        ///     The path to the currently loaded <see cref="TreeContainer" />.
        /// </summary>
        [CanBeNull]
        public string TreeContainerPath { get; private set; }

        /// <summary>
        ///     <para>Gets called when the window opens.</para>
        ///     <para>Populates the window with UI elements and sets them all up.</para>
        /// </summary>
        [UsedImplicitly]
        private void CreateGUI()
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

        /// <summary>
        ///     Gets the currently open instance of the window. If it is not open, open it and return the instance.
        /// </summary>
        /// <returns>A instance of the editor window.</returns>
        public static BehaviourTreeEditor GetOrOpen()
        {
            return _instance != null ? _instance : CreateWindow();
        }

        /// <summary>
        ///     Loads a <see cref="EditorTreeContainer" /> in the editor.
        /// </summary>
        /// <param name="container">The container to load.</param>
        private void LoadTree(EditorTreeContainer container)
        {
            TreeContainer = container;
            TreeView.PopulateView();
            Blackboard.UpdateBlackboard();
        }

        /// <summary>
        ///     Resets the editor window, effectively unloading all editor data.
        /// </summary>
        public void UnloadTree()
        {
            TreeContainer = null;
            TreeContainerPath = null;

            //Recreate the UI.
            rootVisualElement.Clear();
            CreateGUI();
        }

        /// <summary>
        ///     Creates a new instance of the editor window.
        /// </summary>
        /// <returns>A instance of the editor window.</returns>
        [MenuItem("Window/AI/Behaviour Tree Editor")]
        public static BehaviourTreeEditor CreateWindow()
        {
            var wnd = GetWindow<BehaviourTreeEditor>("Behaviour Tree", true, typeof(SceneView));
            wnd.titleContent = new GUIContent("Behaviour Tree", TreeEditorUtility.GetEditorIcon());
            _instance = wnd;
            return wnd;
        }

        /// <summary>
        ///     Hides the <see cref="_curtain" /> and makes the <see cref="_splitView" /> visible.
        /// </summary>
        private void RemoveCurtain()
        {
            _curtain.AddToClassList("hide");
            _splitView.RemoveFromClassList("hide");
        }

        /// <summary>
        ///     <para>
        ///         Gets called when a asset is opened in the unity editor.
        ///     </para>
        ///     <para>
        ///         Determines if the asses is a <see cref="EditorTreeContainer" />. If so, loads it.
        ///     </para>
        /// </summary>
        /// <param name="instanceId">The id of the object instance that is currently trying to be opened.</param>
        /// <param name="line">Unknown. Unity documentation does not mention the use of this value.</param>
        /// <returns>A bool indicating whether the open was handled.</returns>
        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceId, int line)
        {
            var assetPath = AssetDatabase.GetAssetPath(instanceId);
            var treeContainer = AssetDatabase.LoadAssetAtPath<EditorTreeContainer>(assetPath);
            if (treeContainer != null)
            {
                var editor = GetOrOpen();
                editor.TreeContainerPath = assetPath;
                editor.LoadTree(treeContainer);
            }

            return false;
        }
    }
}