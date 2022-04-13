using System;
using System.Collections.Generic;
using System.Linq;
using BehaviourTrees.Core;
using BehaviourTrees.Model;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviourTrees.UnityEditor.UIElements
{
    public sealed class NodeView : Node
    {
        private static EditorTreeContainer Container => BehaviourTreeEditor.GetOrOpen().TreeContainer;
        public readonly NodeModel Node;
        private bool _isMoving;

        private long _timeSinceLastMove;
        public Port InputPort;
        public List<Port> OutputPorts;

        public NodeView(NodeModel node)
            : base(TreeEditorUtility.LocateUiDefinitionFile(nameof(NodeView)))
        {
            Node = node;
            OutputPorts = new List<Port>();
            title = TreeEditorUtility.GetMemberName(Node.RepresentingType);
            viewDataKey = node.Id;

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

        public override void OnSelected()
        {
            base.OnSelected();
            SelectionChanged?.Invoke();
        }

        public override void OnUnselected()
        {
            base.OnUnselected();
            SelectionChanged?.Invoke();
        }

        public void UpdatePorts()
        {
            var openPorts = OutputPorts.Where(port => port.connected is false).ToList();
            if (openPorts.Count > 1)
                foreach (var portToRemove in openPorts.Skip(1))
                    RemovePort(portToRemove);
            else if (Node.RepresentingType.InheritsFrom<CompositeNode>() && openPorts.Count == 0) AddOutputPort();

            outputContainer.Sort(SortPorts);

            if (Node.RepresentingType.InheritsFrom<CompositeNode>())
            {
                var num = 1;
                foreach (var visualElement in outputContainer.Children())
                    if (visualElement is Port port)
                        port.portName = $"{num++}";
            }
        }

        public Port AddOutputPort()
        {
            var port = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, null);
            OutputPorts.Add(port);
            outputContainer.Add(port);
            UpdatePorts();
            return port;
        }

        public Port GetFreeOutputPort()
        {
            return OutputPorts.FirstOrDefault(port => port.connected is false) ?? AddOutputPort();
        }

        public void SetPosition(Vector2 position)
        {
            SetPosition(new Rect(position, Vector2.zero));
        }

        public override void SetPosition(Rect newPos)
        {
            style.position = Position.Absolute;
            style.left = newPos.x;
            style.top = newPos.y;

            _timeSinceLastMove = 0;
            if (_isMoving is false)
            {
                _isMoving = true;
                schedule.Execute(EnsureMovingIsComplete);
            }
        }

        public Edge ConnectTo(NodeView childView)
        {
            var port = GetFreeOutputPort();
            return port.ConnectTo(childView.InputPort);
        }

        private void SetNodeStyle()
        {
            if (Node.RepresentingType == typeof(RootNode))
                AddToClassList("rootNode");
            else if (Node.RepresentingType.InheritsFrom<CompositeNode>())
                AddToClassList("compositeNode");
            else if (Node.RepresentingType.InheritsFrom<DecoratorNode>())
                AddToClassList("decoratorNode");
            else if (Node.RepresentingType.InheritsFrom(typeof(LeafNode<>))) AddToClassList("leafNode");
        }

        private void AddInputPorts()
        {
            if (Node.RepresentingType == typeof(RootNode)) return;

            var port = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, null);
            InputPort = port;
            inputContainer.Add(port);
        }

        private void AddOutputPorts()
        {
            if (Node.RepresentingType.InheritsFrom(typeof(LeafNode<>))) return;

            AddOutputPort();
        }

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

        private void RemovePort(Port port)
        {
            port.DisconnectAll();
            OutputPorts.Remove(port);
            outputContainer.Remove(port);
        }

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
            Container.ModelExtension.NodePositions[Node.Id] = new Vector2(pos.x, pos.y);
            _isMoving = false;
            Container.MarkDirty();
        }

        public event Action SelectionChanged;
    }
}