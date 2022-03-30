namespace BehaviourTrees.Core
{
    /// <summary>
    ///     The base class for decorator nodes.
    /// </summary>
    public abstract class DecoratorNode : BehaviourTreeNode
    {
        public IBehaviourTreeNode Child;
    }
}