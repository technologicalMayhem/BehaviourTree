using System.Collections.Generic;
using BehaviourTrees.Model;
using Newtonsoft.Json;
using UnityEngine;

namespace BehaviourTrees.UnityEditor.Data
{
    /// <summary>
    ///     Represents a <see cref="NodeModel" /> exported from the behaviour tree editor.
    /// </summary>
    public class ExportedNode
    {
        /// <summary>
        ///     The index of the type of this <see cref="NodeModel" /> in the <see cref="ExportedData.Types" /> list.
        /// </summary>
        [JsonProperty("Id")] public int TypeId;

        /// <summary>
        ///     The position of the <see cref="NodeModel" />.
        /// </summary>
        [JsonProperty("Pos")] public Vector2 Position = Vector2.zero;

        /// <summary>
        ///     The <see cref="NodeModel.Properties" /> of the <see cref="NodeModel" />.
        /// </summary>
        [JsonProperty("Props")] public Dictionary<string, object> Properties = new Dictionary<string, object>();

        /// <summary>
        ///     The position of the <see cref="NodeModel" />.
        /// </summary>
        [JsonProperty("Sub")] public List<ExportedNode> Children = new List<ExportedNode>();

        /// <summary>
        ///     Indicates whether or not <see cref="Position" /> should be serialized.
        /// </summary>
        /// <returns>True if it should be serialized.</returns>
        public bool ShouldSerializePosition()
        {
            return Position != Vector2.zero;
        }

        /// <summary>
        ///     Indicates whether or not <see cref="Properties" /> should be serialized.
        /// </summary>
        /// <returns>True if it should be serialized.</returns>
        public bool ShouldSerializeProperties()
        {
            return Properties.Count > 0;
        }

        /// <summary>
        ///     Indicates whether or not <see cref="Children" /> should be serialized.
        /// </summary>
        /// <returns>True if it should be serialized.</returns>
        public bool ShouldSerializeChildren()
        {
            return Children.Count > 0;
        }
    }
}