using System;
using JetBrains.Annotations;

namespace BehaviourTrees.UnityEditor
{
    /// <summary>
    ///     <para>Indicates to the editor that this fields value is a key to retrieve a value from the blackboard.</para>
    ///     <para>
    ///         <b>The field must be of type <see cref="string" /> for this to work.</b>
    ///     </para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    [UsedImplicitly]
    public class BlackboardDropdownAttribute : Attribute
    {
        /// <summary>
        ///     The type the blackboard value returned must be.
        /// </summary>
        public readonly Type Type;

        /// <summary>
        ///     Creates a new instance of the attribute.
        /// </summary>
        /// <param name="type">The type the blackboard value returned must be.</param>
        public BlackboardDropdownAttribute(Type type)
        {
            Type = type;
        }
    }
}