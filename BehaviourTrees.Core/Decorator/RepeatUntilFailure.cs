namespace BehaviourTrees.Core.Decorator
{
    /// <summary>
    ///     Runs it's child until it returns a failure.
    /// </summary>
    public class RepeatUntilFailure : DecoratorNode
    {
        protected override NodeStatus OnUpdate()
        {
            return Child.Update() != NodeStatus.Failure ? NodeStatus.Running : NodeStatus.Success;
        }
    }
}