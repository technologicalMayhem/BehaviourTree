using System.Collections.Generic;

namespace BehaviourTrees.Model
{
    /// <summary>
    /// Represents the behaviour tree.
    /// </summary>
    public sealed class BehaviourTreeModel
    {
        /// <summary>
        /// A dictionary containing the one to many parent/children relationship between nodes.
        /// </summary>
        public readonly Dictionary<string, IList<string>> Connections = new Dictionary<string, IList<string>>();
        /// <summary>
        /// A list of all the nodes in the behaviour tree.
        /// </summary>
        public readonly List<NodeModel> Nodes = new List<NodeModel>();
    }
}