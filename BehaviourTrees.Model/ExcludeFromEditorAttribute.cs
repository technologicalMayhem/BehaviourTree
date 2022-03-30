using System;

namespace BehaviourTrees.Model
{
    [AttributeUsage(AttributeTargets.Field)]
    internal sealed class ExcludeFromEditorAttribute : Attribute { }
}