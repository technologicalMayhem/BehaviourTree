using System;

namespace BehaviourTrees.Core.Blackboard
{
    /// <summary>
    ///     Flags used to indicate the allowed access to a value on the blackboard.
    /// </summary>
    [Flags]
    public enum BlackboardAccess
    {
        /// <summary>
        ///     Read access is allowed.
        /// </summary>
        Read = 1,

        /// <summary>
        ///     Write access is allowed.
        /// </summary>
        Write = 2
    }
}