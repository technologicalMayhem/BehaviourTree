using System.Collections.Generic;

namespace BehaviourTrees.Model
{
    public sealed class ConceptualBehaviourTree
    {
        public readonly Dictionary<string, IList<string>> Connections = new Dictionary<string, IList<string>>();
        public readonly List<NodeModel> Nodes = new List<NodeModel>();
    }
}