using System;
using System.Collections.Generic;
using System.Linq;
using BehaviourTrees.Core;
using BehaviourTrees.UnityEditor.UIElements;
using Newtonsoft.Json;
using UnityEngine;

namespace BehaviourTrees.UnityEditor.Data
{
    /// <summary>
    ///     Represents data exported from the behaviour tree editor.
    /// </summary>
    public class ExportedData
    {
        /// <summary>
        ///     The nodes that have been exported.
        /// </summary>
        public List<ExportedNode> Nodes = new List<ExportedNode>();

        /// <summary>
        ///     List of all the types in this <see cref="ExportedData" /> object.
        /// </summary>
        [JsonConverter(typeof(TypeListConverter))]
        public List<Type> Types = new List<Type>();

        /// <summary>
        ///     Creates <see cref="ExportedData" /> from elements in the behaviour tree editor.
        /// </summary>
        /// <param name="nodeViews">The node view elements.</param>
        /// <param name="nodeRelationships">A string/IList&lt;string&gt; dictionary of node parent/children relationships.</param>
        /// <returns>A object representing data that can be serialized to be exported.</returns>
        public static ExportedData CreateFromNodeViews(IEnumerable<NodeView> nodeViews,
            Dictionary<string, IList<string>> nodeRelationships)
        {
            var nodes = nodeViews
                .Where(view => view.Node.RepresentingType != typeof(RootNode))
                .ToArray();

            var uniqueTypes = nodes
                .Select(view => view.Node.RepresentingType)
                .Distinct()
                .ToList();

            var nodeDictionary = CreateExportedNodes(nodes, uniqueTypes);
            SetupNodeRelationships(nodeDictionary, nodeRelationships);
            var topLevelNodes = GetTopLevelNodes(nodeDictionary.Values.ToList());
            MoveNodesToCenter(topLevelNodes, topLevelNodes.First().Position);

            return new ExportedData
            {
                Nodes = topLevelNodes,
                Types = uniqueTypes
            };
        }

        /// <summary>
        ///     Creates a the <see cref="ExportedNode" />s from the <see cref="NodeView" /> elements.
        /// </summary>
        /// <param name="nodes">The nodes to create nodes for.</param>
        /// <param name="types">A list of the types the nodes use.</param>
        /// <returns>A Id/Node dictionary. The id corresponds to the <see cref="NodeView" />s id.</returns>
        private static Dictionary<string, ExportedNode> CreateExportedNodes(IEnumerable<NodeView> nodes,
            IList<Type> types)
        {
            var keyValuePairs = nodes.Select(view => new KeyValuePair<string, ExportedNode>(view.Node.Id,
                new ExportedNode
                {
                    TypeId = types.IndexOf(view.Node.RepresentingType),
                    Position = view.GetPosition().position,
                    Properties = view.Node.Properties
                }));
            return new Dictionary<string, ExportedNode>(keyValuePairs);
        }

        /// <summary>
        ///     Sets up the parent/children relationships for the nodes.
        /// </summary>
        /// <param name="nodes">A Id/Node dictionary of the nodes.</param>
        /// <param name="nodeRelationships">A string/IList&lt;string&gt; dictionary of node parent/children relationships.</param>
        private static void SetupNodeRelationships(Dictionary<string, ExportedNode> nodes,
            IReadOnlyDictionary<string, IList<string>> nodeRelationships)
        {
            foreach (var (parentId, parent) in nodes)
            {
                nodeRelationships.TryGetValue(parentId, out var childrenIds);
                if (childrenIds == null) continue;
                foreach (var childId in childrenIds)
                {
                    nodes.TryGetValue(childId, out var child);
                    if (child == null) continue;
                    parent.Children.Add(child);
                }
            }
        }

        /// <summary>
        ///     Returns a list of all the nodes that are not children of other nodes.
        /// </summary>
        /// <param name="nodes">The nodes to search.</param>
        /// <returns>A list with the nodes that are not children of other nodes.</returns>
        private static List<ExportedNode> GetTopLevelNodes(IList<ExportedNode> nodes)
        {
            return nodes
                .Where(thisNode =>
                    nodes.Any(otherNode => otherNode.Children.Contains(thisNode)) == false)
                .ToList();
        }

        /// <summary>
        ///     Shifts the nodes and all their children by the given offset.
        /// </summary>
        /// <param name="topLevelNodes">The nodes to shift their position of.</param>
        /// <param name="position">The offset to move them by.</param>
        private static void MoveNodesToCenter(List<ExportedNode> topLevelNodes, Vector2 position)
        {
            foreach (var node in topLevelNodes)
            {
                node.Position -= position;
                MoveNodesToCenter(node.Children, position);
            }
        }

        /// <summary>
        ///     Converts the type list in a simple, version independent format.
        /// </summary>
        private class TypeListConverter : JsonConverter<List<Type>>
        {
            /// <inheritdoc />
            public override void WriteJson(JsonWriter writer, List<Type> value, JsonSerializer serializer)
            {
                writer.WriteStartArray();
                foreach (var type in value) writer.WriteValue(type.FullName);
                writer.WriteEndArray();
            }

            /// <inheritdoc />
            public override List<Type> ReadJson(JsonReader reader, Type objectType, List<Type> existingValue,
                bool hasExistingValue,
                JsonSerializer serializer)
            {
                var list = new List<Type>();

                reader.Read();
                do
                {
                    var t = reader.Value as string;
                    list.Add(TypeLocater.GetTypeByName(t));
                    reader.Read();
                } while (reader.TokenType != JsonToken.EndArray);

                return list;
            }
        }
    }
}