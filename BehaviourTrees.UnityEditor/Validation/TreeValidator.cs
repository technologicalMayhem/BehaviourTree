using System.Collections.Generic;

namespace BehaviourTrees.UnityEditor.Validation
{
    public abstract class TreeValidator
    {
        public abstract IEnumerable<ValidationResult> Validate(EditorTreeContainer container);
    }
}