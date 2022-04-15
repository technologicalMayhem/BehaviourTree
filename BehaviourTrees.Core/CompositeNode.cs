using System.Collections.Generic;

namespace BehaviourTrees.Core
{
    /// <summary>
    ///     Base class for composite nodes.
    /// </summary>
    public abstract class CompositeNode : BehaviourTreeNode
    {
        /// <summary>
        ///     The children of the node.
        /// </summary>
        public IReadOnlyList<IBehaviourTreeNode> Children;
    }
}