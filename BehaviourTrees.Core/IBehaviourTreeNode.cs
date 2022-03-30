namespace BehaviourTrees.Core
{
    public interface IBehaviourTreeNode
    {
        /// <summary>
        ///     A unique id identifying this node.
        /// </summary>
        string Id { get; set; }

        /// <summary>
        ///     <p>Reset the node and clear up runtime data.</p>
        ///     <p>
        ///         This should only be called when the node did not finish execution if, for example, in the meantime other work
        ///         has
        ///         been performed and the node is expected start over.
        ///     </p>
        /// </summary>
        void Reset();

        /// <summary>
        ///     This will tick the execution of the node once.
        /// </summary>
        /// <returns>The execution status of the node.</returns>
        NodeStatus Update();
    }
}