using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Linq;
using BehaviourTrees.Core;
using BehaviourTrees.Core.Blackboard;

namespace BehaviourTrees.Model
{
    /// <summary>
    ///     Provides methods to use a <see cref="BehaviourTreeModel" /> to construct a
    ///     <see cref="BehaviourTree{TContext}" />.
    /// </summary>
    public static class BehaviourTreeConstructor
    {
        /// <summary>
        ///     Construct a <see cref="BehaviourTree{TContext}" /> from the model.
        /// </summary>
        /// <param name="model">The model to use.</param>
        /// <param name="context">A instance of the context the behaviour tree should be using.</param>
        /// <typeparam name="TContext">The type of the context.</typeparam>
        /// <returns>The constructed <see cref="BehaviourTree{TContext}" /></returns>
        [Pure]
        public static BehaviourTree<TContext> ConstructExecutableTree<TContext>(this BehaviourTreeModel model,
            TContext context)
        {
            var nodes = CreateNodes(model);
            LinkUpNodes(model, nodes);
            PopulateNodesWithData(model, nodes, context);

            var blackboard = new BehaviourTreeBlackboard();
            SetupBlackboard(blackboard, nodes);

            var readOnlyCollection = new Collection<IBehaviourTreeNode>(nodes);
            return new BehaviourTree<TContext>(blackboard, readOnlyCollection, context);
        }

        /// <summary>
        ///     Creates the nodes and assigns their ids to them.
        /// </summary>
        /// <param name="treeModel">The tree model to create nodes for.</param>
        /// <returns>A list of the created nodes.</returns>
        /// <exception cref="ArgumentException">
        ///     Gets thrown if a behaviour nodes type does not
        ///     implement <see cref="IBehaviourTreeNode" />.
        /// </exception>
        private static IList<IBehaviourTreeNode> CreateNodes(BehaviourTreeModel treeModel)
        {
            var nodes = new List<IBehaviourTreeNode>();

            foreach (var modelNode in treeModel.Nodes)
            {
                var instance = Activator.CreateInstance(modelNode.RepresentingType);
                if (instance is IBehaviourTreeNode node)
                {
                    node.Id = modelNode.Id;
                    nodes.Add(node);
                }
                else
                {
                    throw new ArgumentException(
                        $"Error whilst instancing node {modelNode.Id}: Type {modelNode.RepresentingType} cannot be assigned to {typeof(IBehaviourTreeNode)}.");
                }
            }

            return nodes;
        }

        /// <summary>
        ///     Populate the nodes with data from the <see cref="BehaviourTreeModel" /> and add a context instance to the
        ///     leaf nodes.
        /// </summary>
        /// <param name="treeModel">The behaviour tree model to use.</param>
        /// <param name="nodes">The nodes to add data to.</param>
        /// <param name="context">The context to be added to leaf nodes.</param>
        /// <typeparam name="TContext">The context type.</typeparam>
        private static void PopulateNodesWithData<TContext>(BehaviourTreeModel treeModel,
            IEnumerable<IBehaviourTreeNode> nodes, TContext context)
        {
            foreach (var node in nodes)
            {
                var model = treeModel.Nodes.First(model => model.Id == node.Id);
                var fields = node.GetFillableFields().ToArray();

                foreach (var keyValuePair in model.Properties)
                {
                    var fieldInfo = fields.First(info => info.Name == keyValuePair.Key);
                    //Make sure that we do potentially necessary casts (example: float could get deserialized as double)
                    var value = Convert.ChangeType(keyValuePair.Value, fieldInfo.FieldType);
                    fieldInfo.SetValue(node, value);
                }

                var contextField = fields.FirstOrDefault(info => info.Name == "Context");
                if (contextField != null) contextField.SetValue(node, context);
            }
        }

        /// <summary>
        ///     Links up the nodes according to the connection in the <see cref="BehaviourTreeModel" />.
        /// </summary>
        /// <param name="treeModel">The BehaviourTreeModel to use.</param>
        /// <param name="nodes">The nodes to be linked up.</param>
        private static void LinkUpNodes(BehaviourTreeModel treeModel, IList<IBehaviourTreeNode> nodes)
        {
            foreach (var keyValuePair in treeModel.Connections)
            {
                var parentUid = keyValuePair.Key;
                var childrenIds = keyValuePair.Value;

                var parent = nodes.First(node => node.Id == parentUid);
                var children = childrenIds.Select(id => nodes.First(node => node.Id == id)).ToList();

                switch (parent)
                {
                    case CompositeNode composite:
                        composite.Children = children;
                        break;
                    case RootNode root:
                        root.Child = children.First();
                        break;
                }
            }
        }

        /// <summary>
        ///     Setups up the blackboard in the behaviour nodes.
        /// </summary>
        /// <param name="behaviourTreeBlackboard">The blackboard to use.</param>
        /// <param name="nodes">The behaviour nodes to set up.</param>
        private static void SetupBlackboard(BehaviourTreeBlackboard behaviourTreeBlackboard,
            IEnumerable<IBehaviourTreeNode> nodes)
        {
            // ReSharper disable once SuspiciousTypeConversion.Global
            foreach (var registersToBlackboard in nodes.OfType<IRegistersToBlackboard>())
                behaviourTreeBlackboard.ProvideBlackboardAccess(registersToBlackboard);
        }
    }
}