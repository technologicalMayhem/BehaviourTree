namespace BehaviourTrees.Core
{
    /// <summary>
    ///     The base class for decorator nodes.
    /// </summary>
    public abstract class DecoratorNode : BehaviourTreeNode
    {
        /// <summary>
        ///     The nodes child.
        /// </summary>
        public IBehaviourTreeNode Child;
    }
}