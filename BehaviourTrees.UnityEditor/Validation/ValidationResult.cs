using JetBrains.Annotations;

namespace BehaviourTrees.UnityEditor.Validation
{
    public struct ValidationResult
    {
        public Severity Severity;
        [CanBeNull] public string NodeId;
        [NotNull] public string Message;
    }
}