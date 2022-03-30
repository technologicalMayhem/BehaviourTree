using System;

namespace BehaviourTrees.Model
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class NodeNameAttribute : Attribute
    {
        public readonly string Name;

        public NodeNameAttribute(string name)
        {
            Name = name;
        }
    }
}