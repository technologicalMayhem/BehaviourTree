using System.Collections.Generic;
using BehaviourTrees.UnityEditor.Data;

namespace BehaviourTrees.UnityEditor.Validation
{
    /// <summary>
    ///     A abstract class to implement a validator for <see cref="EditorTreeContainer" />.
    /// </summary>
    public abstract class TreeValidator
    {
        /// <summary>
        ///     Checks for potential problems in a <see cref="EditorTreeContainer" /> instance.
        /// </summary>
        /// <param name="container">The container to check.</param>
        /// <returns>All potential problems found in the <see cref="EditorTreeContainer" /> instance.</returns>
        public abstract IEnumerable<ValidationResult> Validate(EditorTreeContainer container);
    }
}