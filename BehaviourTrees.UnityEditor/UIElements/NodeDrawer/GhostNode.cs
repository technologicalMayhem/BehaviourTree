using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviourTrees.UnityEditor.UIElements.NodeDrawer
{
    /// <summary>
    ///     A node to be dragged around with the mouse cursor. If clicked on the <see cref="BehaviourTreeView" /> it creates a
    ///     new node in it.
    /// </summary>
    public class GhostNode : FauxNode
    {
        private readonly GhostNodeDragAndDropManipulator _manipulator;
        
        /// <summary>
        ///     Creates a new ghost node element.
        /// </summary>
        /// <param name="nodeType">The node type this node represents.</param>
        /// <param name="evt">The mouse event to attach the node to.</param>
        /// <param name="nodePosition">The position to create the node at.</param>
        /// <param name="nodeSize">The size of the node.</param>
        public GhostNode(Type nodeType, PointerDownEvent evt, Vector3 nodePosition, Vector3 nodeSize) : base(nodeType)
        {
            style.position = new StyleEnum<Position>(Position.Absolute);
            transform.position = nodePosition - new Vector3(nodeSize.x / 2, nodeSize.y / 2);

            _manipulator = new GhostNodeDragAndDropManipulator(this);
        }

        /// <summary>
        ///     Insert an external <see cref="PointerDownEvent" /> into the ghost nodes manipulator.
        /// </summary>
        /// <param name="evt">The event to handle.</param>
        public void HandleExternalPointerDown(PointerDownEvent evt)
        {
            _manipulator.HandleExternalPointerDownInternal(evt);
        }

        /// <summary>
        ///     Obtains all the <see cref="DrawerNode" />s in the sidebar.
        /// </summary>
        /// <returns>All the <see cref="DrawerNode" />s in the sidebar.</returns>
        private static IEnumerable<DrawerNode> GetDrawerNodes()
        {
            return BehaviourTreeEditor.GetOrOpen().Sidebar.Q<NodeDrawer>().Q("container").Children().Cast<DrawerNode>();
        }

        /// <summary>
        ///     A manipulator for handling the movement and click interaction of <see cref="GhostNode" />.
        /// </summary>
        private class GhostNodeDragAndDropManipulator : PointerManipulator
        {
            /// <summary>
            ///     The integer used to represent the left mouse button.
            /// </summary>
            private const int LeftMouseButton = 0;

            /// <summary>
            ///     The integer used to represent the right mouse button.
            /// </summary>
            private const int RightMouseButton = 1;

            /// <summary>
            ///     True if the drag and drop is currently in process
            /// </summary>
            private bool _enabled;

            /// <summary>
            ///     The start position of the target.
            /// </summary>
            private Vector2 _targetStartPosition;

            /// <summary>
            ///     The start position of the pointer.
            /// </summary>
            private Vector3 _pointerStartPosition;

            /// <summary>
            ///     Creates a new instance of the manipulator.
            /// </summary>
            /// <param name="target">The element to attach it to.</param>
            public GhostNodeDragAndDropManipulator(VisualElement target)
            {
                this.target = target;
            }

            /// <inheritdoc />
            protected override void RegisterCallbacksOnTarget()
            {
                target.RegisterCallback<PointerDownEvent>(PointerDownHandler);
                target.RegisterCallback<PointerMoveEvent>(PointerMoveHandler);
                target.RegisterCallback<PointerUpEvent>(PointerUpHandler);
            }


            /// <inheritdoc />
            protected override void UnregisterCallbacksFromTarget()
            {
                target.UnregisterCallback<PointerDownEvent>(PointerDownHandler);
                target.UnregisterCallback<PointerMoveEvent>(PointerMoveHandler);
                target.UnregisterCallback<PointerUpEvent>(PointerUpHandler);
            }

            /// <summary>
            ///     Uses an external <see cref="PointerDownEvent" /> in this manipulator.
            /// </summary>
            /// <param name="evt">The even to use.</param>
            public void HandleExternalPointerDownInternal(PointerDownEvent evt)
            {
                PointerDownHandler(evt);
            }

            /// <summary>
            ///     Handle the <see cref="PointerDownEvent" />.
            /// </summary>
            /// <param name="evt">The event.</param>
            private void PointerDownHandler(PointerDownEvent evt)
            {
                switch (evt.button)
                {
                    case LeftMouseButton when _enabled is false:
                        _targetStartPosition = target.transform.position;
                        _pointerStartPosition = evt.position;
                        target.CapturePointer(evt.pointerId);
                        _enabled = true;
                        break;

                    case LeftMouseButton when _enabled && IsCenterInElement(BehaviourTreeEditor.GetOrOpen().TreeView):
                        var treeView = BehaviourTreeEditor.GetOrOpen().TreeView;
                        var mousePosition = treeView.GetWorldPosition(new Vector2(target.transform.position.x,
                            target.transform.position.y - 20));
                        treeView.CreateNode(((GhostNode)target).NodeType,
                            mousePosition.x, mousePosition.y);
                        break;

                    case LeftMouseButton when _enabled && IsCenterInElementAny(GetDrawerNodes(), out var drawerNode):
                        ReleaseAndRemove(evt.pointerId);

                        drawerNode.CreateGhostNode(evt);
                        break;
                }
            }

            /// <summary>
            ///     Handle the <see cref="PointerMoveEvent" />.
            /// </summary>
            /// <param name="evt">The event.</param>
            private void PointerMoveHandler(PointerMoveEvent evt)
            {
                if (_enabled && target.HasPointerCapture(evt.pointerId))
                {
                    var pointerDelta = evt.position - _pointerStartPosition;

                    target.transform.position = new Vector2(
                        Mathf.Clamp(_targetStartPosition.x + pointerDelta.x, 0,
                            target.panel.visualTree.worldBound.width),
                        Mathf.Clamp(_targetStartPosition.y + pointerDelta.y, 0,
                            target.panel.visualTree.worldBound.height));
                }
            }

            /// <summary>
            ///     Handle the <see cref="PointerUpEvent" />.
            /// </summary>
            /// <param name="evt">The event.</param>
            private void PointerUpHandler(PointerUpEvent evt)
            {
                if (evt.button == RightMouseButton && _enabled && target.HasPointerCapture(evt.pointerId))
                    ReleaseAndRemove(evt.pointerId);
            }

            /// <summary>
            ///     Releases the target from the pointer and remove it from it's parent.
            /// </summary>
            /// <param name="pointerId">The id of the pointer.</param>
            private void ReleaseAndRemove(int pointerId)
            {
                target.ReleasePointer(pointerId);
                target.parent.Remove(target);
            }

            /// <summary>
            ///     Check if the center of the target is inside the element.
            /// </summary>
            /// <param name="element">The element to check if the center is inside of.</param>
            /// <returns>True if the center is inside the element.</returns>
            private bool IsCenterInElement(VisualElement element)
            {
                return element.worldBound.Contains(target.worldBound.center);
            }

            /// <summary>
            ///     Check if the center of the target is inside any of the elements.
            /// </summary>
            /// <param name="elements">A list of elements tho check.</param>
            /// <param name="matchingElement">The first element that the check succeeded on.</param>
            /// <typeparam name="T">The type of the elements.</typeparam>
            /// <returns>True if the center is inside any of the elements.</returns>
            private bool IsCenterInElementAny<T>(IEnumerable<T> elements, out T matchingElement) where T : VisualElement
            {
                foreach (var visualElement in elements)
                    if (IsCenterInElement(visualElement))
                    {
                        matchingElement = visualElement;
                        return true;
                    }

                matchingElement = null;
                return false;
            }
        }
    }
}