using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using BehaviourTrees.UnityEditor.UIElements;

namespace BehaviourTrees.UnityEditor.Data.Events
{
    /// <summary>
    ///     Provides data for the <see cref="NodeView.ConnectionsChanged" /> event.
    /// </summary>
    public class ConnectionChangedEventArgs : EventArgs
    {
        /// <summary>
        ///     All nodes that have been newly connected to the node.
        /// </summary>
        public readonly IReadOnlyList<NodeView> ConnectedNodes;

        /// <summary>
        ///     All nodes that have been disconnected from the node.
        /// </summary>
        public readonly IReadOnlyList<NodeView> DisconnectedNodes;

        /// <summary>
        ///     Create a new instance of <see cref="ConnectionChangedEventArgs" />.
        /// </summary>
        /// <param name="connectedNodes">All nodes that have been newly connected to the node.</param>
        /// <param name="disconnectedNodes">All nodes that have been disconnected from the node.</param>
        public ConnectionChangedEventArgs(IEnumerable<NodeView> connectedNodes, IEnumerable<NodeView> disconnectedNodes)
        {
            ConnectedNodes = new ReadOnlyCollection<NodeView>(connectedNodes.ToList());
            DisconnectedNodes = new ReadOnlyCollection<NodeView>(disconnectedNodes.ToList());
        }
    }
}