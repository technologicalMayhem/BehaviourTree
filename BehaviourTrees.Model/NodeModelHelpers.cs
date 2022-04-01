using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace BehaviourTrees.Model
{
    /// <summary>
    /// This class provides extension methods to help editing a <see cref="NodeModel"/>.
    /// </summary>
    public static class NodeModelHelpers
    {
        /// <summary>
        /// Checks if the models properties still reflect the ones of the type it is representing.
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
            removedProperties = new List<string>();
            changedProperties = new List<string>();
            addedProperties = new List<string>();

            throw new NotImplementedException();

            return removedProperties.Any() || changedProperties.Any() || addedProperties.Any();
        }

        /// <summary>
        /// <p>Makes changes to a node models properties so they line up with it's representing type.</p>
        /// <p>Changes that will be made can be checked with <see cref="ValidateProperties"/> beforehand.</p>
        /// </summary>
        /// <param name="model">The model to update the properties of.</param>
        /// <param name="destructive">Should missing properties be deleted and changed replaced with default values?</param>
        /// <exception cref="NotImplementedException"></exception>
        public static void UpdateProperties(this NodeModel model, bool destructive = false)
        {
            throw new NotImplementedException();
        }
    }
}