using System.Collections.Generic;

namespace BehaviourTrees.UnityEditor.Inspector
{
    /// <summary>
    ///     Provides support for this element to be modified by the inspector.
    /// </summary>
    public interface IEditable
    {
        /// <summary>
        ///     Get information about all editable properties on the object.
        /// </summary>
        /// <returns>A collection of <see cref="PropertyInfo" /> about all editable properties.</returns>
        public IEnumerable<PropertyInfo> GetProperties();

        /// <summary>
        ///     Get the value of a property.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The value of the property.</returns>
        public object GetValue(string propertyName);

        /// <summary>
        ///     Set the value of a property.
        /// </summary>
        /// <param name="propertyName">The property to set the value of.</param>
        /// <param name="value">The value to be assigned to the property.</param>
        public void SetValue(string propertyName, object value);
    }
}