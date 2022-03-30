namespace BehaviourTrees.Core
{
    /// <summary>
    ///     The base class for leaf nodes.
    /// </summary>
    /// <typeparam name="TContext">The type of context to be used.</typeparam>
    public abstract class LeafNode<TContext> : BehaviourTreeNode
    {
        public TContext Context;
    }
}