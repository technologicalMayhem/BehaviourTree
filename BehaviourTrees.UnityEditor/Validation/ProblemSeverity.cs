namespace BehaviourTrees.UnityEditor.Validation
{
    /// <summary>
    ///     The severity of the problem found.
    /// </summary>
    public enum ProblemSeverity
    {
        /// <summary>
        ///     Indicates that this <b>will</b> cause a exceptions if the tree were to be constructed or that it will not function
        ///     correctly during execution.
        /// </summary>
        Error,

        /// <summary>
        ///     Indicates that something <b>might</b> cause problems during editing or execution of the tree.
        /// </summary>
        Warning
    }
}