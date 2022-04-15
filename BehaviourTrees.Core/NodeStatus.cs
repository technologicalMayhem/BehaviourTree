namespace BehaviourTrees.Core
{
    /// <summary>
    ///     The execution status of a node.
    /// </summary>
    public enum NodeStatus
    {
        /// <summary>
        ///     The node has successfully finished execution.
        /// </summary>
        Success,

        /// <summary>
        ///     The node has finished execution, but failed.
        /// </summary>
        Failure,

        /// <summary>
        ///     The node has not finished execution yet.
        /// </summary>
        Running
    }
}