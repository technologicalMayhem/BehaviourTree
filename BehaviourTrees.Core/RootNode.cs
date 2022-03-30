namespace BehaviourTrees.Core
{
    /// <summary>
    ///     The behaviour tree root node. This needs to be at the base of every behaviour tree.
    /// </summary>
    public class RootNode : BehaviourTreeNode
    {
        public IBehaviourTreeNode Child;

        /// <summary>
        ///     Tick the behaviour tree once.
        /// </summary>
        /// <returns>Returns Running whilst executing. Returns Running or Failure depending on result of child.</returns>
        protected override NodeStatus OnUpdate()
        {
            return Child.Update();
        }
    }
}