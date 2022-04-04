using System;

namespace BehaviourTrees.UnityEditor
{
    [AttributeUsage(AttributeTargets.Field)]
    public class BlackboardDropdownAttribute : Attribute
    {
        public Type Type;

        public BlackboardDropdownAttribute(Type type)
        {
            Type = type;
        }
    }
}