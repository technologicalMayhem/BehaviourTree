using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviourTrees.UnityEditor.UIElements.NodeDrawer
{
    /// <summary>
    ///     A node in the node drawer. When clicked on creates a ghost node.
    /// </summary>
    public class DrawerNode : FauxNode
    {
        /// <summary>
        ///     Create a new drawer node element.
        /// </summary>
        /// <param name="nodeType">The type this node should represent.</param>
        public DrawerNode(Type nodeType) : base(nodeType)
        {
            AddToClassList("drawer-node");
            RegisterCallback(new EventCallback<PointerDownEvent>(CreateGhostNode));
        }

        /// <summary>
        ///     Creates a ghost node using the give pointer event.
        /// </summary>
        /// <param name="evt">The pointer event to attach the node to.</param>
        public void CreateGhostNode(PointerDownEvent evt)
        {
            if (evt.button != 0) return;
            var rootVisualElement = BehaviourTreeEditor.GetOrOpen().rootVisualElement;
            var ghostNode = new GhostNode(NodeType, evt, evt.position - (Vector3)rootVisualElement.worldBound.position,
                layout.size);
            rootVisualElement.Add(ghostNode);
            ghostNode.HandleExternalPointerDown(evt);
        }
    }
}