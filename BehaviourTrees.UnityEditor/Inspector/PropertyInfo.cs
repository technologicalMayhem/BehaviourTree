using System;
using System.Diagnostics.CodeAnalysis;

namespace BehaviourTrees.UnityEditor.Inspector
{
    /// <summary>
    ///     Contains information about a property that can be edited in an object.
    /// </summary>
    public class PropertyInfo
    {
        /// <summary>
        ///     Creates a new instance of <see cref="PropertyInfo" />.
        /// </summary>
        /// <param name="name">The name of property</param>
        /// <param name="type">The type of the property.</param>
        /// <param name="value">The value of the property.</param>
        /// <param name="prettyName">
        ///     A user-friendly name for the property. Defaults to the name given in <see cref="Name" />,
        ///     split by pascal case rules.
        /// </param>
        /// <param name="categoryName">Optional, but recommended. A category name to place the property under in the inspector.</param>
        /// <param name="categoryOrder">The order of the category in the inspector. Higher numbers will be placed higher.</param>
        public PropertyInfo(string name, Type type, object value, string prettyName = null, string categoryName = null,
            int categoryOrder = 0)
        {
            Name = name;
            FriendlyName = prettyName ?? TreeEditorUtility.SplitPascalCase(name);
            Type = type;
            Value = value;
            CategoryName = categoryName;
            CategoryOrder = categoryOrder;
        }

        /// <summary>
        ///     The name of the property.
        /// </summary>
        public readonly string Name;

        /// <summary>
        ///     A user-friendly name for the property.
        /// </summary>
        public readonly string FriendlyName;

        /// <summary>
        ///     The type of the property.
        /// </summary>
        public readonly Type Type;

        /// <summary>
        ///     The value of the property.
        /// </summary>
        [MaybeNull] public readonly object Value;

        /// <summary>
        ///     Optional, but recommended. A category name to place the property under in the inspector.
        /// </summary>
        [MaybeNull] public readonly string CategoryName;

        /// <summary>
        ///     The order of the category in the inspector. Higher numbers will be placed higher.
        /// </summary>
        public readonly int CategoryOrder;
    }
}