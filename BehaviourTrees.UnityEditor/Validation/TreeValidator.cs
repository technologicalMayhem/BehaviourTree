using System.Collections.Generic;
using BehaviourTrees.Core;

namespace BehaviourTrees.UnityEditor.Validation
{
    public abstract class TreeValidator
    {
        public abstract IEnumerable<ValidationResult> Validate(EditorTreeContainer container);
    }
}