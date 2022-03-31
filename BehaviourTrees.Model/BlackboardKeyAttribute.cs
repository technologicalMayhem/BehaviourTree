using System;

namespace BehaviourTrees.Model
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class BlackboardKeyAttribute : Attribute
    {
        public string Key { get; }

        // See the attribute guidelines at 
        //  http://go.microsoft.com/fwlink/?LinkId=85236
        public BlackboardKeyAttribute(string key)
        {
            Key = key;
        }
    }
}