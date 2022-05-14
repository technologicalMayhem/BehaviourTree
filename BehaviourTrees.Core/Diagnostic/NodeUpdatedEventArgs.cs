using System;

namespace BehaviourTrees.Core.Diagnostic
{
    /// <summary>
    ///     Contains event data for the <see cref="NodeDiagnostics.NodeUpdated" /> event.
    /// </summary>
    public class NodeUpdatedEventArgs : EventArgs
    {
        /// <summary>
        ///     Creates a new instance of the event data object.
        /// </summary>
        /// <param name="node">The node that has updated.</param>
        /// <param name="updateResult">The result of the update.</param>
        public NodeUpdatedEventArgs(BehaviourTreeNode node, NodeStatus updateResult)
        {
            Node = node;
            UpdateResult = updateResult;
        }

        /// <summary>
        ///     The node that updated.
        /// </summary>
        public readonly BehaviourTreeNode Node;

        /// <summary>
        ///     The result of the nodes update.
        /// </summary>
        public readonly NodeStatus UpdateResult;
    }
}