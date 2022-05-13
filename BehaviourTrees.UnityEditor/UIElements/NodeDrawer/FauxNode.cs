using System;
using BehaviourTrees.Core;
using BehaviourTrees.Model;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace BehaviourTrees.UnityEditor.UIElements.NodeDrawer
{
    /// <summary>
    ///     Abstract class for an object imitating the style of <see cref="NodeView" /> without implementing it's
    ///     functionality.
    /// </summary>
    public abstract class FauxNode : VisualElement
    {
        /// <summary>
        ///     The type this node represents.
        /// </summary>
        protected readonly Type NodeType;

        /// <summary>
        ///     The input container.
        /// </summary>
        private readonly VisualElement _input;

        /// <summary>
        ///     The output container.
        /// </summary>
        private readonly VisualElement _output;

        /// <summary>
        ///     Sets up the visual data for the faux node element.
        /// </summary>
        /// <param name="nodeType"></param>
        protected FauxNode(Type nodeType)
        {
            var visualTree = TreeEditorUtility.GetVisualTree(nameof(FauxNode));
            visualTree.CloneTree(this);

            NodeType = nodeType;

            this.Q<Label>("title-label").text = TreeEditorUtility.GetNodeName(NodeType);
            _output = this.Q("output");
            _input = this.Q("input");

            SetNodeStyle();
            CreatePorts();
        }

        /// <summary>
        ///     Sets the color scheme for the node to use.
        /// </summary>
        private void SetNodeStyle()
        {
            if (NodeType == typeof(RootNode))
                AddToClassList("rootNode");
            else if (NodeType.InheritsFrom<CompositeNode>())
                AddToClassList("compositeNode");
            else if (NodeType.InheritsFrom<DecoratorNode>())
                AddToClassList("decoratorNode");
            else if (NodeType.InheritsFrom(typeof(LeafNode<>))) AddToClassList("leafNode");
        }

        /// <summary>
        ///     Creates the ports on the node.
        /// </summary>
        private void CreatePorts()
        {
            if (NodeType == typeof(RootNode))
            {
                _output.Add(NewPort(Direction.Output));
            }
            else if (NodeType.InheritsFrom<CompositeNode>() || NodeType.InheritsFrom<DecoratorNode>())
            {
                _input.Add(NewPort(Direction.Output));
                _output.Add(NewPort(Direction.Output));
            }
            else if (NodeType.InheritsFrom(typeof(LeafNode<>)))
            {
                _input.Add(NewPort(Direction.Output));
            }
        }

        /// <summary>
        ///     Creates a new port.
        /// </summary>
        /// <param name="direction">The direction of the port.</param>
        /// <returns>A new port.</returns>
        private static Port NewPort(Direction direction)
        {
            return Port.Create<Edge>(Orientation.Horizontal, direction, Port.Capacity.Single, null);
        }
    }
}