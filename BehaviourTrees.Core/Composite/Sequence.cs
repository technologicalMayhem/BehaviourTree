using System;

namespace BehaviourTrees.Core.Composite
{
    /// <summary>
    ///     Runs all it's children in order and return <b>Success</b>.
    ///     If a child returns <b>Failure</b> the Sequence will abort and return <b>Failure</b> as well.
    /// </summary>
    public class Sequence : CompositeNode
    {
        private int _currentChild;

        /// <inheritdoc />
        protected override void OnStartup()
        {
            _currentChild = 0;
        }

        /// <inheritdoc />
        protected override NodeStatus OnUpdate()
        {
            while (true)
            {
                var result = Children[_currentChild].Update();
                switch (result)
                {
                    case NodeStatus.Success:
                        _currentChild++;
                        if (Children.Count == _currentChild) return NodeStatus.Success;
                        continue;
                    case NodeStatus.Failure:
                        return NodeStatus.Failure;
                    case NodeStatus.Running:
                        return NodeStatus.Running;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}