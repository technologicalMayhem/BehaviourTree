using System;

namespace BehaviourTrees.Model
{
    /// <summary>
    ///     Indicates that a behaviour tree node should be referred to different name than it's class name.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class NodeNameAttribute : Attribute
    {
        /// <summary>
        ///     The name to be used for the node.
        /// </summary>
        public readonly string Name;

        /// <summary>
        ///     Create a new instance of the NodeNameAttribute.
        /// </summary>
        /// <param name="name">The name to be used for the node.</param>
        public NodeNameAttribute(string name)
        {
            Name = name;
        }
    }
}