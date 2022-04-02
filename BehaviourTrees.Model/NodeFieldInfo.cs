using System;

namespace BehaviourTrees.Model
{
    /// <summary>
    ///     Represents a field of a type that can be assigned during construction, it's type and default value.
    /// </summary>
    public struct NodeFieldInfo
    {
        /// <summary>
        ///     The name of the field.
        /// </summary>
        public string FieldName;

        /// <summary>
        ///     The type of the field.
        /// </summary>
        public Type FieldType;

        /// <summary>
        ///     The default value of the field, if instantiated.
        /// </summary>
        public object DefaultValue;
    }
}