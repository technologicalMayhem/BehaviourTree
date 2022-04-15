namespace BehaviourTrees.Core.Blackboard
{
    /// <summary>
    ///     Allows a node to be discovered and registered with the blackboard.
    /// </summary>
    public interface IRegistersToBlackboard
    {
        /// <summary>
        ///     Allow the node to register itself with the blackboard.
        /// </summary>
        /// <param name="access"></param>
        public void RegisterBlackboardAccess(IBlackboardAccessProvider access);
    }
}