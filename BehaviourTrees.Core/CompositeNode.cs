using System.Collections.Generic;

namespace BehaviourTrees.Core
{
    /// <summary>
    ///     Base class for composite nodes.
    /// </summary>
    public abstract class CompositeNode : BehaviourTreeNode
    {
        public IReadOnlyList<IBehaviourTreeNode> Children;
    }
}