using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using BehaviourTrees.Model;

namespace BehaviourTrees.UnityEditor
{
    /// <summary>
    ///     Finds types ìn assemblies and provides extension methods to filter them.
    /// </summary>
    public static class TypeLocater
    {
        /// <summary>
        ///     Gets all types loaded in the current <see cref="AppDomain" /> or in the supplied assemblies.
        /// </summary>
        /// <param name="assemblies">
        ///     Optional. A collection of assemblies to get all types from.
        ///     If left as null, all types in the current <see cref="AppDomain" /> will be retrieved.
        /// </param>
        /// <returns>A collection of all retrieved types.</returns>
        public static IEnumerable<Type> GetAllTypes(Assembly[] assemblies = null)
        {
            return (assemblies ?? AppDomain.CurrentDomain.GetAssemblies())
                .SelectMany(assembly => assembly.GetTypes());
        }

        /// <summary>
        ///     Finds a type by its full name.
        /// </summary>
        /// <param name="fullName">The full name of the type.</param>
        /// <param name="assemblies">
        ///     Optional. A collection of assemblies to search for the type in.
        ///     If left as null, all types in the current <see cref="AppDomain" /> will be searched.
        /// </param>
        /// <returns>The type matching the given name, or null if none could be found.</returns>
        [return: MaybeNull]
        public static Type GetTypeByName(string fullName, Assembly[] assemblies = null)
        {
            return GetAllTypes(assemblies)
                .FirstOrDefault(type => type.FullName == fullName);
        }

        /// <summary>
        ///     Filters out all types that do not inherit from the given type.
        /// </summary>
        /// <param name="types">The collection of types to filter.</param>
        /// <param name="baseType">The type that needs to be inherited from.</param>
        /// <returns>All types inheriting the given type.</returns>
        public static IEnumerable<Type> ThatInheritFrom(this IEnumerable<Type> types, Type baseType)
        {
            return types.Where(type => type.InheritsFrom(baseType));
        }

        /// <summary>
        ///     Filters all types out that do not inherit from the given type.
        /// </summary>
        /// <param name="types">The collection of types to filter.</param>
        /// <typeparam name="TBaseType">The type that needs to be inherited from.</typeparam>
        /// <returns>All types inheriting the given type.</returns>
        public static IEnumerable<Type> ThatInheritFrom<TBaseType>(this IEnumerable<Type> types)
        {
            return types.Where(ModelUtilities.InheritsFrom<TBaseType>);
        }

        /// <summary>
        ///     Filters out all types that do not have a public parameterless constructor.
        /// </summary>
        /// <param name="types">The collection of types to filter.</param>
        /// <returns>All types that have a default constructor.</returns>
        public static IEnumerable<Type> ThatHaveADefaultConstructor(this IEnumerable<Type> types)
        {
            return types.Where(HasDefaultConstructor);
        }

        /// <summary>
        ///     Checks if a type has a public parameterless constructor.
        /// </summary>
        /// <param name="type">The type to check for a default constructor.</param>
        /// <returns>True if the type has a default constructor.</returns>
        public static bool HasDefaultConstructor(this Type type)
        {
            return type.IsValueType || type.GetConstructors()
                .Any(info => info.IsPublic && !info.IsAbstract && !info.GetParameters().Any());
        }
    }
}