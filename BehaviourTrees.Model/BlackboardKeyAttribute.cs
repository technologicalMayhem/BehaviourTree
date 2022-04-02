using System;

namespace BehaviourTrees.Model
{
    /// <summary>
    /// Indicates that a variable is a blackboard key.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class BlackboardKeyAttribute : Attribute
    {
        /// <summary>
        /// The key of the variable in the blackboard. If this matches a field name then the value of that member will be used as a key instead.
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Create a new instance of the blackboard key attribute.
        /// </summary>
        /// <param name="key">The key of the variable in the blackboard. If this matches a field name then the value of that member will be used as a key instead.</param>
        public BlackboardKeyAttribute(string key)
        {
            Key = key;
        }
    }
}