using System;
using System.Collections.Generic;
using System.Linq;
using BehaviourTrees.UnityEditor.Data;
using BehaviourTrees.UnityEditor.Data.Events;
using BehaviourTrees.UnityEditor.Inspector;
using BehaviourTrees.UnityEditor.Interfaces;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace BehaviourTrees.UnityEditor.UIElements
{
    public class CommentBlock : GraphElement, IEditable, ISelectionCallback
    {
        public readonly NodeView AttachedTo;
        private readonly Label _commentText;

        private readonly List<NodeView> _containedNodes;

        private EditorTreeContainer Container => BehaviourTreeEditor.GetOrOpen().TreeContainer;

        public CommentBlock(NodeView attachedTo)
        {
            var visualTree = TreeEditorUtility.GetVisualTree(nameof(CommentBlock));
            visualTree.CloneTree(this);

            _containedNodes = new List<NodeView>();

            AttachedTo = attachedTo;
            capabilities = Capabilities.Selectable;
            layer = -100;

            this.Q("comment-block");
            _commentText = this.Q<Label>("comment-text");
            if (Container.ModelExtension.Comments.TryGetValue(AttachedTo.Node.Id, out var text))
                _commentText.text = text;
            _commentText.RegisterCallback(new EventCallback<GeometryChangedEvent>(_ => UpdatePositionAndSize()));

            this.AddManipulator(new ContextualMenuManipulator(AddMenuOption));
        }

        private void AddMenuOption(ContextualMenuPopulateEvent evt)
        {
            if (!(evt.target is CommentBlock)) return;

            evt.menu.AppendAction("Remove comment", action =>
            {
                BehaviourTreeEditor.GetOrOpen().TreeView.RemoveElement(this);
                CleanupEventSubscriptions();
                Container.ModelExtension.Comments.Remove(AttachedTo.Node.Id);
            });
            evt.menu.AppendSeparator();
        }

        private void UpdatePositionAndSize()
        {
            var nodeViews = AttachedTo.GetDescendants().Append(AttachedTo);
            var xMin = float.PositiveInfinity;
            var yMin = float.PositiveInfinity;
            var xMax = float.NegativeInfinity;
            var yMax = float.NegativeInfinity;

            CleanupEventSubscriptions();

            foreach (var view in nodeViews)
            {
                RegisterEvents(view);

                var viewLayout = view.layout;
                if (viewLayout.x < xMin) xMin = viewLayout.x;
                if (viewLayout.x + viewLayout.width > xMax) xMax = viewLayout.x + viewLayout.width;
                if (viewLayout.y < yMin) yMin = viewLayout.y;
                if (viewLayout.y + viewLayout.height > yMax) yMax = viewLayout.y + viewLayout.height;
            }

            var textHeight = _commentText.layout.height;

            var position = GetPosition();
            position.x = xMin - 10;
            position.y = yMin - 10 - textHeight;
            position.height = yMax - yMin + 20 + textHeight;
            position.width = xMax - xMin + 20;
            SetPosition(position);
        }

        private void RegisterEvents(NodeView view)
        {
            view.NodeMoved += OnNodeMoved;
            view.ConnectionsChanged += OnConnectionsChanged;
            _containedNodes.Add(view);
        }

        private void OnNodeMoved(object sender, MovedEventArgs e)
        {
            UpdatePositionAndSize();
        }

        private void OnConnectionsChanged(object sender, ConnectionChangedEventArgs e)
        {
            UpdatePositionAndSize();
        }

        public void CleanupEventSubscriptions()
        {
            foreach (var node in _containedNodes)
            {
                node.NodeMoved -= OnNodeMoved;
                node.ConnectionsChanged -= OnConnectionsChanged;
            }

            _containedNodes.Clear();
        }

        /// <inheritdoc />
        public IEnumerable<PropertyInfo> GetProperties()
        {
            return new[] { new PropertyInfo("Comment", typeof(string), _commentText.text) };
        }

        /// <inheritdoc />
        public object GetValue(string propertyName)
        {
            return propertyName switch
            {
                "Comment" => Container.ModelExtension.Comments[AttachedTo.Node.Id],
                _ => throw new ArgumentOutOfRangeException(nameof(propertyName))
            };
        }

        /// <inheritdoc />
        public void SetValue(string propertyName, object value)
        {
            switch (propertyName)
            {
                case "Comment":
                    var s = value as string;
                    _commentText.text = s;
                    Container.ModelExtension.Comments[AttachedTo.Node.Id] = s;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(propertyName));
            }
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

        /// <inheritdoc />
        public event EventHandler Selected;

        /// <inheritdoc />
        public event EventHandler Unselected;
    }
}