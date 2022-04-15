using System;

namespace BehaviourTrees.Core.Decorator
{
    /// <summary>
    ///     Always returns success. Running gets passed on.
    /// </summary>
    public class Success : DecoratorNode
    {
        /// <inheritdoc />
        protected override NodeStatus OnUpdate()
        {
            return Child.Update() switch
            {
                NodeStatus.Success => NodeStatus.Success,
                NodeStatus.Failure => NodeStatus.Success,
                NodeStatus.Running => NodeStatus.Running,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}