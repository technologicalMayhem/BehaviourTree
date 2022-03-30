using System;

namespace BehaviourTrees.Core.Blackboard
{
    /// <summary>
    ///     Flags used to indicate the allowed access to a value on the blackboard.
    /// </summary>
    [Flags]
    public enum BlackboardAccess
    {
        Read = 1,
        Write = 2
    }
}