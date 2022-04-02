using System;
using System.Collections.Generic;

namespace BehaviourTrees.Model
{
    /// <summary>
    ///     Represents a behaviour tree node.
    /// </summary>
    public class NodeModel
    {
        /// <summary>
        ///     A unique identifier to refer to the node as.
        /// </summary>
        public string Id;

        /// <summary>
        ///     The properties of this node. They indicate what field names should be set to which values during construction.
        /// </summary>
        public Dictionary<string, object> Properties;

        /// <summary>
        ///     The node type that this model represents.
        /// </summary>
        public Type RepresentingType;
    }
}