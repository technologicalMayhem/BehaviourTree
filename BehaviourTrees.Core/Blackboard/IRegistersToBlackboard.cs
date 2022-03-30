namespace BehaviourTrees.Core.Blackboard
{
    public interface IRegistersToBlackboard
    {
        /// <summary>
        ///     Allow the node to register itself with the blackboard.
        /// </summary>
        /// <param name="access"></param>
        public void RegisterBlackboardAccess(IBlackboardAccessProvider access);
    }
}