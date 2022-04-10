using System.Collections.Generic;
using System.Linq;
using BehaviourTrees.Core;
using BehaviourTrees.UnityEditor.UIElements;
using UnityEngine;

namespace BehaviourTrees.UnityEditor
{
    public static class TreeFormatter
    {
        public static void FormatTreeStructure(BehaviourTreeView view)
        {
            var rootNode = view.nodes
                .Cast<NodeView>()
                .First(node => node.Node.RepresentingType == typeof(RootNode));
            var alignmentBlockTree = CreateAlignmentBlockTree(rootNode);

            alignmentBlockTree.Align();
            alignmentBlockTree.ApplyPosition();
        }

        private static AlignmentBlock CreateAlignmentBlockTree(NodeView node, AlignmentBlock parent = null)
        {
            var children = GetChildren(node);

            var block = new AlignmentBlock
            {
                Node = node,
                Position = new Rect(0, 0, node.layout.width, node.layout.height)
            };

            block.Children = children.Select(child => CreateAlignmentBlockTree(child, block)).ToList();

            return block;
        }

        private static IEnumerable<NodeView> GetChildren(NodeView node)
        {
            return node.OutputPorts
                .Select(port => port.connections
                    .FirstOrDefault()?.input.node as NodeView)
                .Where(view => view != null);
        }

        private class AlignmentBlock
        {
            public NodeView Node;
            public List<AlignmentBlock> Children;
            public Rect Position;
            private static readonly Vector2 Margin = new Vector2(50, 20);

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

            private Vector2 GetFullSize()
            {
                var size = GetChildrenSize();

                size.x += Position.width + Margin.x;
                if (Position.height > size.y)
                    size.y = Position.height;

                return size;
            }

            private void Move(Vector2 move)
            {
                Position.x += move.x;
                Position.y += move.y;

                foreach (var child in Children) child.Move(move);
            }

            public void ApplyPosition()
            {
                Node.SetPosition(Position);

                foreach (var child in Children) child.ApplyPosition();
            }
        }
    }
}