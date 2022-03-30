using System.Diagnostics;

namespace BehaviourTrees.Core
{
    /// <summary>
    ///     Provides diagnostics methods and events.
    /// </summary>
    public static class BehaviourTreeDebug
    {
        [Conditional("DEBUG")]
        public static void ReportNodeResult(BehaviourTreeNode node, NodeStatus result) { }
    }
}