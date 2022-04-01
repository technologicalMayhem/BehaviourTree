using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using BehaviourTrees.Core;

namespace BehaviourTrees.Model
{
    /// <summary>
    /// This class provides extension methods to help editing a <see cref="BehaviourTreeModel"/>.
    /// </summary>
    public static class TreeModelHelpers
    {
        /// <summary>
        /// Creates a new node and adds it to the tree model.
        /// </summary>
        /// <param name="treeModel">The tree model to create the node in.</param>
        /// <typeparam name="T">The type of node.</typeparam>
        /// <returns>The newly created node.</returns>
        /// <exception cref="ArgumentException">Will be thrown if the node type is not assignable to <see cref="IBehaviourTreeNode"/></exception>
        public static NodeModel CreateNode<T>(this BehaviourTreeModel treeModel) => CreateNode(treeModel, typeof(T));

        /// <summary>
        /// Creates a new node and adds it to the tree model.
        /// </summary>
        /// <param name="treeModel">The tree model to create the node in.</param>
        /// <param name="type">The type of node.</param>
        /// <returns>The newly created node.</returns>
        /// <exception cref="ArgumentException">Will be thrown if the node type is not assignable to <see cref="IBehaviourTreeNode"/></exception>
        public static NodeModel CreateNode(this BehaviourTreeModel treeModel, Type type)
        {
            if (!type.InheritsFrom(typeof(IBehaviourTreeNode)))
                throw new ArgumentException(
                    $"Type must be inheriting from {typeof(IBehaviourTreeNode)}, but {type} doesn't.");
            var model = new NodeModel
            {
                RepresentingType = type,
                Id = Utility.CreateShortId()
            };
            model.UpdateProperties();
            treeModel.Nodes.Add(model);

            return model;
        }

        /// <summary>
        /// Removes a node from the tree.
        /// </summary>
        /// <param name="treeModel">The model the node should be removed from.</param>
        /// <param name="node">The node to be removed from the tree.</param>
        public static void RemoveNode(this BehaviourTreeModel treeModel, NodeModel node) =>
            treeModel.Nodes.Remove(node);

        /// <summary>
        /// Adds <paramref name="childNode"/> as a parent to <paramref name="parentNode"/>.
        /// </summary>
        /// <param name="treeModel">The tree model that the relationship should be added to.</param>
        /// <param name="parentNode">The parent of the relationship.</param>
        /// <param name="childNode">The child of the relationship.</param>
        /// <param name="parentPortIndex">The desired position in the parent node.</param>
        /// <exception cref="InvalidOperationException">Will be thrown if the <paramref name="parentNode"/> is a
        /// <see cref="LeafNode{TContext}"/> or the <paramref name="childNode"/> is a <see cref="RootNode"/>.</exception>
        public static void AddChild(this BehaviourTreeModel treeModel, NodeModel parentNode, NodeModel childNode,
            int parentPortIndex = -1)
        {
            if (childNode.RepresentingType == typeof(RootNode))
                throw new InvalidOperationException("Root node can't have a parent.");

            if (parentNode.RepresentingType.InheritsFrom(typeof(LeafNode<>)))
                throw new InvalidOperationException("Leaf nodes cannot have children.");

            if (treeModel.Connections.TryGetValue(parentNode.Id, out var list))
            {
                //Make sure the desired index is within range, otherwise add it to the end
                if (parentPortIndex >= 0 && parentPortIndex < list.Count)
                    list.Insert(parentPortIndex, childNode.Id);
                else
                    list.Add(childNode.Id);
            }
            else
            {
                treeModel.Connections[parentNode.Id] = new List<string> { childNode.Id };
            }
        }

        /// <summary>
        /// Removes a child from it's parent. Use <see cref="RemoveChild"/> if the parent is known already, otherwise
        /// it will be looked up.
        /// </summary>
        /// <param name="treeModel">The tree model the relationship is defined in.</param>
        /// <param name="childNode">The child node to be removed from it's parent.</param>
        /// <exception cref="InvalidOperationException">Will be thrown if a root node is passed as <paramref name="childNode"/></exception>
        public static void RemoveParent(BehaviourTreeModel treeModel, NodeModel childNode)
        {
            if (childNode.RepresentingType == typeof(RootNode))
                throw new InvalidOperationException("Root node can't be a child to another node");

            var connections = treeModel.Connections;
            foreach (var valuePair in connections.Where(pair => pair.Value.Contains(childNode.Id)))
            {
                valuePair.Value.Remove(childNode.Id);
                if (!valuePair.Value.Any())
                {
                    connections.Remove(valuePair.Key);
                }
            }
        }

        /// <summary>
        /// Removes a child from it's parent.
        /// </summary>
        /// <param name="treeModel">The tree model the relationship is defined in.</param>
        /// <param name="childNode">The child node to be removed from it's parent.</param>
        /// <param name="parentNode">The parent node.</param>
        /// <exception cref="InvalidOperationException">Will be thrown if a root node is passed as <paramref name="childNode"/></exception>
        public static void RemoveChild(BehaviourTreeModel treeModel, NodeModel childNode, NodeModel parentNode)
        {
            if (childNode.RepresentingType == typeof(RootNode))
                throw new InvalidOperationException("Root node can't be a child to another node");

            var childrenOfParent = treeModel.Connections[parentNode.Id];
            childrenOfParent.Remove(childNode.Id);
            if (!childrenOfParent.Any()) treeModel.Connections.Remove(parentNode.Id);
        }

        /// <summary>
        /// Find a node by it's id.
        /// </summary>
        /// <param name="treeModel">The tree model to search in.</param>
        /// <param name="id">The id of the node to look for.</param>
        /// <returns>The node with the given id or null if no doe with that id exists.</returns>
        [Pure]
        public static NodeModel GetNodeById(this BehaviourTreeModel treeModel, string id) =>
            treeModel.Nodes.FirstOrDefault(model => model.Id == id);

        /// <summary>
        /// Get all children of the node.
        /// </summary>
        /// <param name="treeModel">The tree model containing the relationships.</param>
        /// <param name="parentNode">The node parent node.</param>
        /// <returns>A collection with the children of the parent node.</returns>
        [Pure]
        public static IEnumerable<NodeModel> GetChildren(this BehaviourTreeModel treeModel, NodeModel parentNode)
        {
            var connections = treeModel.Connections;
            if (connections.ContainsKey(parentNode.Id))
                return connections[parentNode.Id]
                    .Select(s => treeModel.Nodes.First(model => model.Id == s));

            return Array.Empty<NodeModel>();
        }
    }
}