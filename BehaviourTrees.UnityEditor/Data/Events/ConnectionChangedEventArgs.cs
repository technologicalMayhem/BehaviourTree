using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using BehaviourTrees.UnityEditor.UIElements;

namespace BehaviourTrees.UnityEditor.Data.Events
{
    public class ConnectionChangedEventArgs : EventArgs
    {
        public readonly IReadOnlyList<NodeView> ConnectedNodes;
        public readonly IReadOnlyList<NodeView> DisconnectedNodes;

        public ConnectionChangedEventArgs(IEnumerable<NodeView> connectedNodes, IEnumerable<NodeView> disconnectedNodes)
        {
            ConnectedNodes = new ReadOnlyCollection<NodeView>(connectedNodes.ToList());
            DisconnectedNodes = new ReadOnlyCollection<NodeView>(disconnectedNodes.ToList());
        }
    }
}