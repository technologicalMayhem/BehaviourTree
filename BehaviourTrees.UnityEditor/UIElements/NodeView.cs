using System;
using System.Collections.Generic;
using System.Linq;
using BehaviourTrees.Core;
using BehaviourTrees.Model;
using BehaviourTrees.UnityEditor.Data;
using BehaviourTrees.UnityEditor.Data.Events;
using BehaviourTrees.UnityEditor.Inspector;
using BehaviourTrees.UnityEditor.Interfaces;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviourTrees.UnityEditor.UIElements
{
    /// <summary>
    ///     Represents a <see cref="NodeModel" /> on the graph view.
    /// </summary>
    public sealed class NodeView : Node, IEditable, ISelectionCallback
    {
        /// <summary>
        ///     A reference to the tree container contained in the main editor window.
        /// </summary>
        private static EditorTreeContainer Container => BehaviourTreeEditor.GetOrOpen().TreeContainer;

        private bool _isConnectionChangeUpdateScheduled;
        private readonly List<NodeView> _nodeConnectionSnapshot;

        /// <summary>
        ///     The <see cref="NodeModel" /> this NodeView is representing.
        /// </summary>
        public readonly NodeModel Node;

        /// <summary>
        ///     Used to keep track of if the node has been moved recently and when to save the new position.
        /// </summary>
        private long _timeSinceLastMove;

        /// <summary>
        ///     The input port on the node.
        /// </summary>
        public Port InputPort;

        /// <summary>
        ///     The output ports on the node.
        /// </summary>
        public readonly List<Port> OutputPorts;

        /// <summary>
        ///     Creates a new instance of a NodeView element.
        /// </summary>
        /// <param name="node">The node to represent.</param>
        public NodeView(NodeModel node)
            : base(TreeEditorUtility.LocateUiDefinitionFile(nameof(NodeView)))
        {
            Node = node;
            OutputPorts = new List<Port>();
            title = TreeEditorUtility.GetNodeName(Node.RepresentingType);
            viewDataKey = node.Id;
            _nodeConnectionSnapshot = new List<NodeView>();

            if (!Container.ModelExtension.NodePositions.TryGetValue(node.Id, out var position))
                position = Vector2.zero;

            style.left = position.x;
            style.top = position.y;

            //Remove default stylesheet
            styleSheets.Clear();

            AddInputPorts();
            AddOutputPorts();
            SetNodeStyle();

            if (node.RepresentingType == typeof(RootNode)) capabilities -= Capabilities.Deletable;
        }

        /// <summary>
        ///     Set node position.
        /// </summary>
        /// <param name="position"></param>
        public void SetPosition(Vector2 position)
        {
            SetPosition(new Rect(position, Vector2.zero));
        }

        /// <inheritdoc />
        public override void SetPosition(Rect newPos)
        {
            style.position = Position.Absolute;
            style.left = newPos.x;
            style.top = newPos.y;

            _timeSinceLastMove = 0;
            schedule.Execute(EnsureMovingIsComplete);
        }

        /// <summary>
        ///     Create a edge between this node and another node on the output side.
        /// </summary>
        /// <param name="childView">The node to connect to.</param>
        /// <returns>The created edge.</returns>
        public Edge ConnectTo(NodeView childView)
        {
            var port = GetUnconnectedOutputPort();
            return port.ConnectTo(childView.InputPort);
        }

        /// <inheritdoc />
        public override void OnSelected()
        {
            base.OnSelected();
            Selected?.Invoke(this, EventArgs.Empty);
        }

        /// <inheritdoc />
        public override void OnUnselected()
        {
            base.OnUnselected();
            Unselected?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        ///     Updates the ports on the node. It add a new port if there is no unconnected one or removes excess
        ///     unconnected ports if there are more than one empty one. Also sorts ports in their execution order and places
        ///     the unconnected port at the bottom.
        /// </summary>
        public void UpdatePorts()
        {
            BeginConnectionUpdateEvent();
            RemoveOrAddPorts();
            outputContainer.Sort(SortPorts);
            UpdatePortNames();
        }

        private void SetComment(string comment)
        {
            tooltip = comment;
            if (string.IsNullOrWhiteSpace(comment))
            {
                Container.ModelExtension.Comments.Remove(Node.Id);
                RemoveFromClassList("comment");
            }
            else
            {
                Container.ModelExtension.Comments[Node.Id] = comment;
                AddToClassList("comment");
            }
        }

        private string GetComment()
        {
            Container.ModelExtension.Comments.TryGetValue(Node.Id, out var comment);
            return comment ?? string.Empty;
        }

        /// <summary>
        ///     <p>Collects a snapshot and schedules the <see cref="ConnectionsChanged" /> update to be raised next frame.</p>
        ///     <p>
        ///         Calling this multiple time in the same frame will not raise the event multiple time or override the snapshot.
        ///         This will only be done the first time.
        ///     </p>
        /// </summary>
        private void BeginConnectionUpdateEvent()
        {
            if (_isConnectionChangeUpdateScheduled == false)
            {
                _isConnectionChangeUpdateScheduled = true;
                _nodeConnectionSnapshot.AddRange(GetChildren());
                schedule.Execute(RaiseConnectionUpdateEvent);
            }
        }

        /// <summary>
        ///     Raises the <see cref="ConnectionsChanged" /> event. Sends the set of changes to all subscribers.
        /// </summary>
        private void RaiseConnectionUpdateEvent()
        {
            var currentNodes = GetChildren().ToArray();

            ConnectionsChanged?.Invoke(this, new ConnectionChangedEventArgs(
                currentNodes.Except(_nodeConnectionSnapshot),
                _nodeConnectionSnapshot.Except(currentNodes)
            ));

            _isConnectionChangeUpdateScheduled = false;
        }

        /// <summary>
        ///     Removes all unused ports but one or adds new a new port.
        /// </summary>
        private void RemoveOrAddPorts()
        {
            var openPorts = OutputPorts.Where(port => port.connected is false).ToList();
            if (openPorts.Count > 1)
                foreach (var portToRemove in openPorts.Skip(1))
                    RemovePort(portToRemove);
            else if (Node.RepresentingType.InheritsFrom<CompositeNode>() && openPorts.Count == 0) AddOutputPort();
        }

        /// <summary>
        ///     Compares two ports and return a int indicating their order.
        /// </summary>
        /// <param name="x">The first port.</param>
        /// <param name="y">The second port.</param>
        /// <returns>
        ///     <c>0</c> if the elements are equal,<c>-1</c> if <paramref name="x" /> is lesser
        ///     and <c>1</c> if <paramref name="x" /> is greater.
        /// </returns>
        private int SortPorts(VisualElement x, VisualElement y)
        {
            if (x is Port portX && y is Port portY)
                switch (portX.connected, portY.connected)
                {
                    case (false, false):
                        //If both are not connected, they are equal
                        return 0;
                    case (true, false):
                        //If x is connected but y is not, x is lesser
                        return -1;
                    case (false, true):
                        //If x is not connected but y is, y is lesser
                        return 1;
                    case (true, true):
                    {
                        //Get the ports corresponding node views
                        var nodeX = (NodeView)portX.connections.First().input.node;
                        var nodeY = (NodeView)portY.connections.First().input.node;

                        //Get their indexes from their parent node
                        var collection = Container.TreeModel.Connections[Node.Id].ToList();
                        var indexX = collection.IndexOf(nodeX.Node.Id);
                        var indexY = collection.IndexOf(nodeY.Node.Id);

                        //If x is smaller than y, so it is earlier in the list, it is lesser
                        return indexX < indexY ? -1 : 1;
                    }
                }

            //If both are not ports they are equal
            return 0;
        }

        /// <summary>
        ///     Update the names of the ports if it is a composite node.
        /// </summary>
        private void UpdatePortNames()
        {
            if (Node.RepresentingType.InheritsFrom<CompositeNode>())
            {
                var num = 1;
                foreach (var visualElement in outputContainer.Children())
                    if (visualElement is Port port)
                        port.portName = $"{num++}";
            }
        }

        /// <summary>
        ///     Gets the first unconnected output port.
        /// </summary>
        /// <returns>A port with no connection.</returns>
        private Port GetUnconnectedOutputPort()
        {
            return OutputPorts.FirstOrDefault(port => port.connected is false) ?? AddOutputPort();
        }

        /// <summary>
        ///     Create a new output port and returns it.
        /// </summary>
        /// <returns>The new output port.</returns>
        private Port AddOutputPort()
        {
            var port = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, null);
            OutputPorts.Add(port);
            outputContainer.Add(port);
            return port;
        }

        /// <summary>
        ///     Removes a port.
        /// </summary>
        /// <param name="port">The port to remove.</param>
        private void RemovePort(Port port)
        {
            port.DisconnectAll();
            OutputPorts.Remove(port);
            outputContainer.Remove(port);
        }

        /// <summary>
        ///     Add a input port to the node, unless it is a root node.
        /// </summary>
        private void AddInputPorts()
        {
            if (Node.RepresentingType == typeof(RootNode)) return;

            var port = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, null);
            InputPort = port;
            inputContainer.Add(port);
        }

        /// <summary>
        ///     Add input ports unless it is a leaf node.
        /// </summary>
        private void AddOutputPorts()
        {
            if (Node.RepresentingType.InheritsFrom(typeof(LeafNode<>))) return;

            AddOutputPort();
        }

        /// <summary>
        ///     Sets a style class on the node depending on what kind of behaviour node the representing type is.
        /// </summary>
        private void SetNodeStyle()
        {
            if (Node.RepresentingType == typeof(RootNode))
                AddToClassList("rootNode");
            else if (Node.RepresentingType.InheritsFrom<CompositeNode>())
                AddToClassList("compositeNode");
            else if (Node.RepresentingType.InheritsFrom<DecoratorNode>())
                AddToClassList("decoratorNode");
            else if (Node.RepresentingType.InheritsFrom(typeof(LeafNode<>))) AddToClassList("leafNode");

            var comment = GetComment();
            if (!string.IsNullOrWhiteSpace(comment))
            {
                tooltip = comment;
                AddToClassList("comment");
            }
        }

        /// <summary>
        ///     Check if moving the node is complete. If not it will retry on the next frame.
        /// </summary>
        /// <param name="state">The timer state of the scheduler.</param>
        private void EnsureMovingIsComplete(TimerState state)
        {
            _timeSinceLastMove += state.deltaTime;

            if (_timeSinceLastMove < 50)
            {
                schedule.Execute(EnsureMovingIsComplete);
                return;
            }

            Undo.RecordObject(Container, "Behaviour Tree (Moved Node)");
            var pos = GetPosition();
            NodeMoved?.Invoke(this,
                new MovedEventArgs(pos.position, Container.ModelExtension.NodePositions[Node.Id]));

            Container.ModelExtension.NodePositions[Node.Id] = new Vector2(pos.x, pos.y);
            Container.MarkDirty();
        }

        /// <summary>
        ///     Raised when the node has moved.
        /// </summary>
        public event EventHandler<MovedEventArgs> NodeMoved;

        /// <summary>
        ///     Raised when the output connections have changed.
        /// </summary>
        public event EventHandler<ConnectionChangedEventArgs> ConnectionsChanged;

        /// <summary>
        ///     Gets the children connected to this <see cref="NodeView" />.
        /// </summary>
        /// <returns>The children of the node.</returns>
        public IEnumerable<NodeView> GetChildren()
        {
            return OutputPorts
                .Where(port => port.connected)
                .Select(port => port.connections
                    .First().input.node as NodeView);
        }

        /// <summary>
        ///     Gets all descendants of this <see cref="NodeView" />.
        /// </summary>
        /// <returns>All descendants of the node.</returns>
        public IEnumerable<NodeView> GetDescendants()
        {
            var descendants = new List<NodeView>();
            var newDescendants = new Queue<NodeView>(GetChildren());

            while (newDescendants.Count > 0)
            {
                var view = newDescendants.Dequeue();

                foreach (var child in view.GetChildren()) newDescendants.Enqueue(child);

                descendants.Add(view);
            }

            return descendants;
        }

        /// <inheritdoc />
        public IEnumerable<PropertyInfo> GetProperties()
        {
            return Node.GetFillableFieldsFromType().Select(info => new PropertyInfo(
                    info.FieldName,
                    info.FieldType,
                    Node.Properties[info.FieldName],
                    categoryName: $"{TreeEditorUtility.GetNodeName(Node.RepresentingType)} Properties"))
                .Append(new PropertyInfo(
                    "Comment",
                    typeof(string),
                    GetComment(),
                    categoryName: "Common Properties",
                    categoryOrder: 100));
        }

        /// <inheritdoc />
        public object GetValue(string propertyName)
        {
            return propertyName switch
            {
                "Comment" => GetComment(),
                _ => Node.Properties[propertyName]
            };
        }

        /// <inheritdoc />
        public void SetValue(string propertyName, object value)
        {
            switch (propertyName)
            {
                case "Comment":
                    SetComment(value as string);
                    break;
                default:
                    Node.Properties[propertyName] = value;
                    break;
            }
        }

        /// <inheritdoc />
        public event EventHandler Selected;

        /// <inheritdoc />
        public event EventHandler Unselected;
    }
}