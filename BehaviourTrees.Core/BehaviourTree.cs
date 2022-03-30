using System;
using System.Collections.Generic;
using System.Linq;
using BehaviourTrees.Core.Blackboard;

namespace BehaviourTrees.Core
{
    /// <summary>
    ///     Represents a behaviour with it's nodes, blackboard and context and allow execution of it.
    /// </summary>
    /// <typeparam name="TContext">The type of the context.</typeparam>
    public class BehaviourTree<TContext>
    {
        private readonly RootNode _rootNode;

        /// <summary>
        ///     Creates a new instance of the <see cref="BehaviourTree{TContext}" /> class.
        /// </summary>
        /// <param name="blackboard">The blackboard to be used.</param>
        /// <param name="nodes">A collection with the nodes of the behaviour tree.</param>
        /// <param name="context">The behaviour trees context.</param>
        /// <exception cref="ArgumentException"></exception>
        public BehaviourTree(BehaviourTreeBlackboard blackboard, IReadOnlyCollection<IBehaviourTreeNode> nodes,
            TContext context)
        {
            Blackboard1 = blackboard;
            Nodes = nodes;
            Context = context;
            _rootNode = Nodes.OfType<RootNode>().FirstOrDefault() ??
                        throw new ArgumentException("Node collection does not contain a root node.");
        }

        /// <summary>
        ///     The behaviour trees blackboard.
        /// </summary>
        public BehaviourTreeBlackboard Blackboard1 { get; }

        /// <summary>
        ///     The nodes that the behaviour tree consists of.
        /// </summary>
        public IReadOnlyCollection<IBehaviourTreeNode> Nodes { get; }

        /// <summary>
        ///     The context that the nodes will use.
        /// </summary>
        public TContext Context { get; }

        /// <summary>
        ///     Executes a tick on the tree.
        /// </summary>
        /// <returns>Indicates whether the tree has finished execution.</returns>
        public bool Tick()
        {
            return _rootNode.Update() == NodeStatus.Running;
        }
    }
}