using System.Diagnostics;

namespace BehaviourTrees.Core
{
    /// <summary>
    ///     Provides diagnostics methods and events.
    /// </summary>
    public static class BehaviourTreeDebug
    {
        /// <summary>
        ///     When a behaviour tree node finishes updating it should publish its state to this method.
        /// </summary>
        /// <param name="node">The node that finished updating.</param>
        /// <param name="result">The result of the update.</param>
        [Conditional("DEBUG")]
        public static void ReportNodeResult(BehaviourTreeNode node, NodeStatus result) { }
    }
}