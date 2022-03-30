namespace BehaviourTrees.Core
{
    /// <summary>
    ///     Base class for all behaviour nodes.
    /// </summary>
    public abstract class BehaviourTreeNode : IBehaviourTreeNode
    {
        /// <summary>
        ///     Tracks whether the node is executing right now.
        /// </summary>
        private bool _isRunning;

        /// <inheritdoc />
        public string Id { get; set; }


        /// <inheritdoc />
        public void Reset()
        {
            OnStartup();
        }

        /// <inheritdoc />
        public NodeStatus Update()
        {
            if (_isRunning is false)
            {
                OnStartup();
                _isRunning = true;
            }

            var result = OnUpdate();
            BehaviourTreeDebug.ReportNodeResult(this, result);
            if (result != NodeStatus.Running) _isRunning = false;
            return result;
        }

        /// <summary>
        ///     <p>
        ///         Define work that needs to be done before running <see cref="Update" /> here.
        ///     </p>
        ///     <p>
        ///         For example:
        ///         <ul>
        ///             <li>Getting data from blackboard or from the context</li>
        ///             <li>Creating objects require for execution</li>
        ///         </ul>
        ///     </p>
        ///     <p>
        ///         Also make sure to clear up runtime data from any previous executions so that when Update is called the node
        ///         is a 'clean slate' again.
        ///     </p>
        /// </summary>
        public virtual void OnStartup() { }

        /// <summary>
        ///     <p>
        ///         Define the work your node will do per tick here.
        ///     </p>
        ///     <p>
        ///         Be aware that even if you return <b>Running</b> you might not run get the opportunity to complete
        ///         your work as the might decide to run other nodes instead, so plan accordingly.
        ///     </p>
        /// </summary>
        /// <returns></returns>
        protected abstract NodeStatus OnUpdate();
    }
}