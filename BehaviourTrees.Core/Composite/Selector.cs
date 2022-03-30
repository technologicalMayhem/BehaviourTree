using System;

namespace BehaviourTrees.Core.Composite
{
    /// <summary>
    ///     Runs it's children in order until one returns <b>Success</b>. The node will then stop execution and return
    ///     <b>Success</b>.
    ///     If none of it's children return <b>Success</b> it will return <b>Failure</b>.
    /// </summary>
    public class Selector : CompositeNode
    {
        private int _currentChild;

        public override void OnStartup()
        {
            _currentChild = 0;
        }

        protected override NodeStatus OnUpdate()
        {
            while (true)
            {
                var result = Children[_currentChild].Update();
                switch (result)
                {
                    case NodeStatus.Success:
                        return NodeStatus.Success;
                    case NodeStatus.Failure:
                        _currentChild++;
                        if (Children.Count == _currentChild) return NodeStatus.Failure;
                        continue;
                    case NodeStatus.Running:
                        return NodeStatus.Running;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}