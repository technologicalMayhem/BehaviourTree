using System.Collections.Generic;
using System.Linq;
using BehaviourTrees.Core;
using BehaviourTrees.UnityEditor.UIElements;
using UnityEngine;

namespace BehaviourTrees.UnityEditor
{
    /// <summary>
    ///     Provides methods to reformat the behaviour tree in a <see cref="BehaviourTreeView" />.
    /// </summary>
    public static class TreeFormatter
    {
        /// <summary>
        ///     Formats the behaviour tree in a more easily parseable structure.
        /// </summary>
        /// <param name="view">The view containing the behaviour tree.</param>
        public static void FormatTreeStructure(BehaviourTreeView view)
        {
            var rootNode = view.nodes
                .Cast<NodeView>()
                .First(node => node.Node.RepresentingType == typeof(RootNode));
            var alignmentBlockTree = CreateAlignmentBlockTree(rootNode);

            alignmentBlockTree.Align();
            alignmentBlockTree.ApplyPosition();
        }

        /// <summary>
        ///     Creates <see cref="AlignmentBlock" />s for the node and all its children.
        /// </summary>
        /// <param name="node">The node to create alignment blocks for.</param>
        /// <returns>The alignment block of the node.</returns>
        private static AlignmentBlock CreateAlignmentBlockTree(NodeView node)
        {
            var children = GetChildren(node);

            var block = new AlignmentBlock
            {
                Node = node,
                Position = new Rect(0, 0, node.layout.width, node.layout.height)
            };

            block.Children = children.Select(CreateAlignmentBlockTree).ToList();

            return block;
        }

        /// <summary>
        ///     Gets the children connected to a <see cref="NodeView" />.
        /// </summary>
        /// <param name="node">The node to get the children of.</param>
        /// <returns>The <see cref="NodeView" />s of the children of the node.</returns>
        private static IEnumerable<NodeView> GetChildren(NodeView node)
        {
            return node.OutputPorts
                .Select(port => port.connections
                    .FirstOrDefault()?.input.node as NodeView)
                .Where(view => view != null);
        }

        /// <summary>
        ///     <para>Represents a set of nodes during the alignment process.</para>
        ///     <para>
        ///         These blocks represent a node in the behaviour tree and all its children in the view. Its stores data
        ///         about their sizes and positions and calculates how they should be laid out as to not collide with one another.
        ///     </para>
        /// </summary>
        private class AlignmentBlock
        {
            public NodeView Node;
            public List<AlignmentBlock> Children;
            public Rect Position;
            private static readonly Vector2 Margin = new Vector2(50, 20);

            /// <summary>
            ///     Positions the alignment block and all of its children in such a manner that they are laid out according
            ///     to their hierarchical structure without colliding with one another.
            /// </summary>
            public void Align()
            {
                foreach (var child in Children) child.Align();

                var move = new Vector2(Position.width + Margin.x, 0);

                foreach (var child in Children)
                {
                    child.Move(move);
                    move.y += child.GetFullSize().y + Margin.y;
                }

                //Center
                if (Children.Any())
                {
                    var childrenSize = GetChildrenSize();
                    var centerMove = childrenSize.y / 2 - Position.height / 2;

                    Position.y += centerMove;
                }
            }

            /// <summary>
            ///     Get the height and width of alignment blocks children.
            /// </summary>
            /// <returns>The height and width of the children. X width and Y is height.</returns>
            private Vector2 GetChildrenSize()
            {
                var blockWidth = 0f;
                var blockHeight = 0f;
                foreach (var size in Children.Select(child => child.GetFullSize()))
                {
                    if (size.x > blockWidth)
                        blockWidth = size.x;
                    blockHeight += size.y + Margin.y;
                }

                return new Vector2(blockWidth, blockHeight);
            }

            /// <summary>
            ///     Gets the full size of the alignment block with all of its children.
            /// </summary>
            /// <returns>The full size of the alignment block.</returns>
            private Vector2 GetFullSize()
            {
                var size = GetChildrenSize();

                size.x += Position.width + Margin.x;
                if (Position.height > size.y)
                    size.y = Position.height;

                return size;
            }

            /// <summary>
            ///     Move this alignment block and all of its children.
            /// </summary>
            /// <param name="move">How much the block should be moved.</param>
            private void Move(Vector2 move)
            {
                Position.x += move.x;
                Position.y += move.y;

                foreach (var child in Children) child.Move(move);
            }

            /// <summary>
            ///     Applies all the calculated positions onto the <see cref="NodeView" />s.
            /// </summary>
            public void ApplyPosition()
            {
                Node.SetPosition(Position);

                foreach (var child in Children) child.ApplyPosition();
            }
        }
    }
}