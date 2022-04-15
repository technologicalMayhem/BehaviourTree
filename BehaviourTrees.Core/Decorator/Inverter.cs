using System;

namespace BehaviourTrees.Core.Decorator
{
    /// <summary>
    ///     <p>Inverts the result of a node. Running gets passed on.</p>
    ///     <p>
    ///         Success -> Failure<br />
    ///         Failure -> Success<br />
    ///         Running -> Running
    ///     </p>
    /// </summary>
    public class Inverter : DecoratorNode
    {
        /// <inheritdoc />
        protected override NodeStatus OnUpdate()
        {
            return Child.Update() switch
            {
                NodeStatus.Success => NodeStatus.Failure,
                NodeStatus.Failure => NodeStatus.Success,
                NodeStatus.Running => NodeStatus.Running,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}