using BehaviourTrees.UnityEditor.Data;
using JetBrains.Annotations;

namespace BehaviourTrees.UnityEditor.Validation
{
    /// <summary>
    ///     Represents a problem found in the <see cref="EditorTreeContainer" />/.
    /// </summary>
    public struct ValidationResult
    {
        /// <summary>
        ///     The severity of the problem.
        /// </summary>
        public ProblemSeverity Severity;

        /// <summary>
        ///     The id of the node the problem is associated with, if any.
        /// </summary>
        [CanBeNull] public string NodeId;

        /// <summary>
        ///     A message describing the problem.
        /// </summary>
        [NotNull] public string Message;
    }
}