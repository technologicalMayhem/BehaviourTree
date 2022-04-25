using System;
using System.Collections.Generic;
using System.Linq;
using BehaviourTrees.Core;
using BehaviourTrees.Model;
using BehaviourTrees.UnityEditor.Data;
using Newtonsoft.Json;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviourTrees.UnityEditor.UIElements
{
    //Todo: Runtime States
    //Todo: Runtime Editing?
    /// <summary>
    ///     A editor for making changes to <see cref="BehaviourTreeModel" /> using a graph view.
    /// </summary>
    public class BehaviourTreeView : GraphView
    {
        /// <summary>
        ///     Creates a new instance of a tree view for editing behaviour trees.
        /// </summary>
        public BehaviourTreeView()
        {
            Insert(0, new GridBackground());

            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            //Remove default stylesheet
            styleSheets.Clear();

            unserializeAndPaste = UnserializeAndPaste;
            serializeGraphElements = SerializeNodes;

            Undo.undoRedoPerformed += OnUndoRedo;
        }

        /// <summary>
        ///     Serializes the nodes into a shareable format.
        /// </summary>
        /// <param name="elements">A collection of elements to serialize.</param>
        /// <returns>A string that can be used to paste nodes into the behaviour tree editor.</returns>
        private static string SerializeNodes(IEnumerable<GraphElement> elements)
        {
            var nodeViews = elements
                .Where(element => element is NodeView)
                .Cast<NodeView>();

            var data = ExportedData.CreateFromNodeViews(nodeViews, TreeModel.Connections);
            return JsonConvert.SerializeObject(data, TreeEditorUtility.Settings);
        }

        /// <summary>
        ///     Deserializes and pastes the given data into the behaviour tree editor.
        /// </summary>
        /// <param name="operationName">The name of the operation. Unused by this method.</param>
        /// <param name="data">A string containing a serialized <see cref="ExportedData" /> object.</param>
        /// <exception cref="ArgumentNullException">
        ///     Will be thrown if <paramref name="data" /> cannot be serialized into a
        ///     <see cref="ExportedData" /> object.
        /// </exception>
        private void UnserializeAndPaste(string operationName, string data)
        {
            var exportedData = JsonConvert.DeserializeObject<ExportedData>(data);
            if (exportedData == null)
                throw new ArgumentException($"The data could not be deserialized into a {nameof(ExportedData)} object.",
                    nameof(data));
            var viewportCenter = new Vector2(viewTransform.position.x, viewTransform.position.y) * -1 + layout.size / 2;

            NodeModel PasteNode(ExportedNode node)
            {
                var btNode = TreeModel.CreateNode(exportedData!.Types[node.TypeId]);
                TreeContainer.ModelExtension.NodePositions[btNode.Id] = viewportCenter + node.Position;
                btNode.Properties = node.Properties;

                foreach (var child in node.Children) TreeModel.AddChild(btNode, PasteNode(child));

                return btNode;
            }

            foreach (var node in exportedData.Nodes) PasteNode(node);

            //Rebuild the view. Reflects our changes in the easiest way.
            PopulateView();
        }

        /// <summary>
        ///     The tree model in the tree container.
        /// </summary>
        private static BehaviourTreeModel TreeModel => TreeContainer.TreeModel;

        /// <summary>
        ///     A reference to the tree container contained in the main editor window.
        /// </summary>
        private static EditorTreeContainer TreeContainer => BehaviourTreeEditor.GetOrOpen().TreeContainer;

        /// <inheritdoc />
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            var worldMousePosition = GetWorldMousePosition(evt);
            var menu = evt.menu;

            AddDerivedTypesToContextMenu(typeof(CompositeNode), menu, worldMousePosition);
            menu.AppendSeparator();
            AddDerivedTypesToContextMenu(typeof(DecoratorNode), menu, worldMousePosition);
            menu.AppendSeparator();
            AddDerivedTypesToContextMenu(typeof(LeafNode<>), menu, worldMousePosition);
        }

        /// <inheritdoc />
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return ports.ToList().Where(
                    endPort => endPort.direction != startPort.direction &&
                               endPort.node != startPort.node)
                .ToList();
        }

        /// <summary>
        ///     Moves the viewport to the node with the given id.
        /// </summary>
        /// <param name="nodeId">The id of the node to move to.</param>
        public void MoveTo(string nodeId)
        {
            MoveTo(TreeContainer.ModelExtension.NodePositions[nodeId]);
        }

        /// <summary>
        ///     Move the viewport to the position.
        /// </summary>
        /// <param name="position">The position to move the viewport to.</param>
        public void MoveTo(Vector2 position)
        {
            //Todo: Figure out why this works the way it does and try to make this clearer
            var rect = contentViewContainer.parent.contentRect;
            var viewportCenter = new Vector2(rect.width / 2, rect.height / 2);
            var centeredPosition = position * -1 + viewportCenter;

            UpdateViewTransform(new Vector3(centeredPosition.x, centeredPosition.y, 0), Vector3.one);
        }

        /// <summary>
        ///     Gets the position of the mouse cursor on the GraphView element.
        /// </summary>
        /// <param name="evt">The event holding the mouse position.</param>
        /// <returns>The position of the cursor.</returns>
        private Vector2 GetWorldMousePosition(IMouseEvent evt)
        {
            //Code from: http://answers.unity.com/answers/1853975/view.html
            var screenMousePosition = evt.localMousePosition;
            var transformPosition = contentViewContainer.transform.position;
            var worldMousePosition = screenMousePosition - new Vector2(transformPosition.x, transformPosition.y);
            var scaleX = 1 / contentViewContainer.transform.scale.x;
            worldMousePosition *= scaleX;
            return worldMousePosition;
        }

        /// <summary>
        ///     Adds all behaviour nodes to the context menu that derive from the given type with an action to create it at the
        ///     current mouse position.
        /// </summary>
        /// <param name="menu">The context menu to be populated.</param>
        /// <param name="mousePosition">The current position of the mouse cursor.</param>
        /// <param name="baseType">The type from which the behaviour nodes must derive from.</param>
        private void AddDerivedTypesToContextMenu(Type baseType, DropdownMenu menu, Vector2 mousePosition)
        {
            foreach (var derivedType in TypeCache.GetTypesDerivedFrom(baseType))
                menu.AppendAction(TreeEditorUtility.GetNodeName(derivedType),
                    _ => CreateNode(derivedType, mousePosition.x, mousePosition.y));
        }

        /// <summary>
        ///     Creates a new node. Optionally accepts a position for the new node.
        /// </summary>
        /// <param name="type">The type of the node to create.</param>
        /// <param name="posX">The X position of the new node.</param>
        /// <param name="posY">The Y position of the new node.</param>
        private void CreateNode(Type type, float posX = 0, float posY = 0)
        {
            Undo.RecordObject(TreeContainer, "Create Node");
            var node = TreeModel.CreateNode(type);
            TreeContainer.ModelExtension.NodePositions[node.Id] = new Vector2(posX, posY);
            CreateNodeView(node);
            TreeContainer.MarkDirty();
        }

        /// <summary>
        ///     Gets called when an undo or redo is performed.
        /// </summary>
        private void OnUndoRedo()
        {
            if (TreeContainer != null) PopulateView();
        }

        /// <summary>
        ///     Clears the view and populates it with all the elements from the given <see cref="EditorTreeContainer" /> to allow
        ///     editing.
        /// </summary>
        public void PopulateView()
        {
            if (TreeContainer != null) Undo.ClearUndo(TreeContainer);
            
            graphViewChanged -= OnGraphViewChanged;
            DeleteElements(graphElements);
            graphViewChanged += OnGraphViewChanged;


            //Create Node views
            foreach (var node in TreeModel.Nodes) CreateNodeView(node);

            //Create Edges
            foreach (var connection in TreeModel.Connections)
            {
                var parentId = connection.Key;
                foreach (var childId in connection.Value)
                {
                    var parentView = GetNodeByGuid(parentId) as NodeView;
                    var childView = GetNodeByGuid(childId) as NodeView;

                    var edge = parentView?.ConnectTo(childView);
                    AddElement(edge);
                }
            }

            if (TreeModel.Nodes.All(node => node.RepresentingType != typeof(RootNode))) CreateNode(typeof(RootNode));

            foreach (var element in graphElements)
                if (element is NodeView node)
                    node.UpdatePorts();

            TreeLoaded?.Invoke();
        }

        /// <summary>
        ///     Make the corresponding edits to the <see cref="BehaviourTreeModel" /> when graph elements are being changed.
        /// </summary>
        /// <param name="graphViewChange">Set of changes in the graph.</param>
        /// <returns>Set of changes in the graph</returns>
        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            if (graphViewChange.elementsToRemove != null)
                ProcessRemovedElements(graphViewChange.elementsToRemove);

            if (graphViewChange.edgesToCreate != null)
                ProcessCreatedEdges(graphViewChange.edgesToCreate);

            TreeContainer.MarkDirty();
            return graphViewChange;
        }

        private void ProcessRemovedElements(List<GraphElement> removedElements)
        {
            foreach (var element in removedElements)
                switch (element)
                {
                    case NodeView nodeView:
                        Undo.RecordObject(TreeContainer, "Delete Node");
                        TreeModel.RemoveNode(nodeView.Node);
                        break;
                    case Edge edge:
                        Undo.RecordObject(TreeContainer, "Delete Connection");
                        var parentView = (NodeView)edge.output.node;
                        var childView = (NodeView)edge.input.node;

                        TreeModel.RemoveChild(childView.Node, parentView.Node);
                        //We need to update the ports on the next frame as right now our updates have not propagated properly
                        schedule.Execute(() => parentView.UpdatePorts());
                        break;
                }
        }

        private void ProcessCreatedEdges(List<Edge> createdEdges)
        {
            foreach (var edge in createdEdges)
                if (edge.output.node is NodeView parentView && edge.input.node is NodeView childView)
                {
                    Undo.RecordObject(TreeContainer, "Create Connection");
                    var parentPortIndex = edge.output.node.outputContainer.IndexOf(edge.output);

                    TreeModel.AddChild(parentView.Node, childView.Node, parentPortIndex);
                    schedule.Execute(() => parentView.UpdatePorts());
                }
        }

        /// <summary>
        ///     Create a new <see cref="NodeView" /> in the graph for the given <see cref="NodeModel" />.
        /// </summary>
        /// <param name="node">The NodeModel that a NodeView should be created for.</param>
        private void CreateNodeView(NodeModel node)
        {
            var nodeView = new NodeView(node);
            nodeView.SelectionChanged += (sender, args) => schedule.Execute(OnSelectionChanged);
            AddElement(nodeView);
        }

        /// <summary>
        ///     Gets the currently selected node and fires the <see cref="SelectionChanged" /> action.
        /// </summary>
        private void OnSelectionChanged()
        {
            var selectedNodes = selection.Where(selectable => selectable is NodeView).Cast<NodeView>().ToList();
            if (selectedNodes.Count > 1)
            {
                SelectionChanged?.Invoke(null);
                return;
            }

            SelectionChanged?.Invoke(selectedNodes.FirstOrDefault());
        }

        /// <summary>
        ///     Gets called when the selection of nodes has changed. Returns null when no nodes or multiple are selected.
        /// </summary>
        public event Action<NodeView> SelectionChanged;

        /// <summary>
        ///     Gets called once a behaviour tree has been successfully loaded into the view.
        /// </summary>
        public event Action TreeLoaded;

        /// <summary>
        ///     Instantiates a <see cref="BehaviourTreeView" /> using the data read from a UXML file.
        /// </summary>
        public new class UxmlFactory : UxmlFactory<BehaviourTreeView, UxmlTraits> { }
    }
}