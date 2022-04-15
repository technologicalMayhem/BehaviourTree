namespace BehaviourTrees.Core
{
    /// <summary>
    ///     The base class for leaf nodes.
    /// </summary>
    /// <typeparam name="TContext">The type of context to be used.</typeparam>
    public abstract class LeafNode<TContext> : BehaviourTreeNode
    {
        /// <summary>
        ///     The context allows nodes to retrieve information about and manipulate the environment that the
        ///     behaviour tree is being run in.
        /// </summary>
        public TContext Context;
    }
}