using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BehaviourTrees.Core;

namespace BehaviourTrees.Model
{
    /// <summary>
    ///     A set of utilities to help with editing the models.
    /// </summary>
    public static class ModelUtilities
    {
        /// <summary>
        ///     Tests if the given type inherits from the other.
        /// </summary>
        /// <param name="type">The type to check if it inherits from the given base type.</param>
        /// <typeparam name="TBaseType">The base type.</typeparam>
        /// <returns></returns>
        public static bool InheritsFrom<TBaseType>(this Type type)
        {
            return InheritsFrom(type, typeof(TBaseType));
        }

        /// <summary>
        ///     Tests if the given type inherits from the other.
        /// </summary>
        /// <param name="type">The type to check if it inherits from the given base type.</param>
        /// <param name="baseType">The base type to check against.</param>
        /// <returns></returns>
        public static bool InheritsFrom(this Type type, Type baseType)
        {
            if (type == null) return false;
            if (baseType.IsAssignableFrom(type))
                return true;
            if (type.IsInterface && !baseType.IsInterface)
                return false;
            if (baseType.IsInterface)
                return type.GetInterfaces().Contains(baseType);
            for (var type1 = type; type1 != null; type1 = type1.BaseType)
                if (type1 == baseType || baseType.IsGenericTypeDefinition && type1.IsGenericType &&
                    type1.GetGenericTypeDefinition() == baseType)
                    return true;
            return false;
        }

        /// <summary>
        ///     Find all fields that can be assigned during construction.
        /// </summary>
        /// <param name="node">The node to lookup the fields for.</param>
        /// <returns>A collection of the fields that can be assigned.</returns>
        public static IEnumerable<FieldInfo> GetFillableFields(
            this IBehaviourTreeNode node)
        {
            return node.GetType().GetFields();
        }
    }
}