using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using BehaviourTrees.Core;

namespace BehaviourTrees.Model
{
    /// <summary>
    ///     This class provides extension methods to help editing a <see cref="NodeModel" />.
    /// </summary>
    public static class NodeModelHelpers
    {
        /// <summary>
        ///     Checks if the models properties still reflect the ones of the type it is representing.
        /// </summary>
        /// <param name="model">The model to validate.</param>
        /// <param name="removedProperties">The list of properties that have been removed.</param>
        /// <param name="changedProperties">The list of properties that have changed in type.</param>
        /// <param name="addedProperties"></param>
        /// <returns>A bool indicating if any problems have been detected.</returns>
        [Pure]
        public static bool ValidateProperties(this NodeModel model,
            out IEnumerable<string> removedProperties,
            out IEnumerable<string> changedProperties,
            out IEnumerable<string> addedProperties)
        {
            var typeProperties = model.GetFillableFieldsFromType().ToArray();
            var modelProperties = model.Properties;

            removedProperties = modelProperties.Keys.Except(typeProperties.Select(info => info.FieldName));
            addedProperties = typeProperties.Select(info => info.FieldName).Except(model.Properties.Keys);

            changedProperties = typeProperties
                .Where(info => modelProperties.ContainsKey(info.FieldName))
                .Where(info => info.FieldType != modelProperties[info.FieldName].GetType())
                .Select(info => info.FieldName);

            return removedProperties.Any() || changedProperties.Any() || addedProperties.Any();
        }

        /// <summary>
        ///     <p>Makes changes to a node models properties so they line up with it's representing type.</p>
        ///     <p>Changes that will be made can be checked with <see cref="ValidateProperties" /> beforehand.</p>
        /// </summary>
        /// <param name="model">The model to update the properties of.</param>
        /// <param name="destructive">Should missing properties be deleted and changed replaced with default values?</param>
        /// <exception cref="NotImplementedException"></exception>
        public static void UpdateProperties(this NodeModel model, bool destructive = false)
        {
            var changesRequired = model.ValidateProperties(
                out var removedProperties,
                out var changedProperties,
                out var addedProperties);

            if (!changesRequired) return;

            var fieldInfos = model.GetFillableFieldsFromType().ToArray();

            foreach (var property in addedProperties)
            {
                var fieldInfo = fieldInfos.First(info => info.FieldName == property);
                model.Properties.Add(fieldInfo.FieldName, fieldInfo.DefaultValue);
            }

            if (!destructive) return;
            var changedPropertiesArray = changedProperties as string[] ?? changedProperties.ToArray();

            foreach (var property in removedProperties.Concat(changedPropertiesArray))
                model.Properties.Remove(property);

            foreach (var property in changedPropertiesArray)
            {
                var fieldInfo = fieldInfos.First(info => info.FieldName == property);
                model.Properties.Add(fieldInfo.FieldName, fieldInfo.DefaultValue);
            }
        }

        /// <summary>
        ///     Gets information about the fields of the representing type of a node model that can be filled during
        ///     construction of the behaviour tree.
        /// </summary>
        /// <param name="model">The model that contain the representing type.</param>
        /// <returns>Information about the fields that can be filled.</returns>
        [Pure]
        public static IEnumerable<NodeFieldInfo> GetFillableFieldsFromType(this NodeModel model)
        {
            var fieldInfos = GetFieldsWithoutBaseTypeFields(model.RepresentingType);

            var instance = Activator.CreateInstance(model.RepresentingType);
            return fieldInfos.Select(info => new NodeFieldInfo
            {
                FieldName = info.Name,
                FieldType = info.FieldType,
                DefaultValue = info.GetValue(instance)
            });
        }

        private static IEnumerable<FieldInfo> GetFieldsWithoutBaseTypeFields(Type type)
        {
            var fieldInfos = type.GetFields()
                .Where(info =>
                {
                    //It should not never be null i think, but if is it's not what we are looking for anyway
                    if (info.DeclaringType == null) return true;
                    //If the field has been defined in Leaf node we don't want
                    if (info.DeclaringType.IsConstructedGenericType)
                    {
                        return info.DeclaringType.GetGenericTypeDefinition() != typeof(LeafNode<>);
                    }

                    //If the field has been defined in any of these we also don't want it.
                    return !new[] { typeof(RootNode), typeof(CompositeNode), typeof(DecoratorNode) }
                        .Contains(info.DeclaringType);
                });

            return fieldInfos;
        }
    }
}