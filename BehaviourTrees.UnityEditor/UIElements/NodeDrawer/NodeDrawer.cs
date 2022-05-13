using System.Linq;
using BehaviourTrees.Core;
using BehaviourTrees.Model;
using UnityEngine.UIElements;

namespace BehaviourTrees.UnityEditor.UIElements.NodeDrawer
{
    /// <summary>
    ///     Shows nodes that can be placed in the <see cref="BehaviourTreeView" />. Nodes can be picked up and placed in the
    ///     tree view.
    /// </summary>
    public class NodeDrawer : SidebarElement
    {
        /// <summary>
        ///     Creates a new node drawer element.
        /// </summary>
        public NodeDrawer()
        {
            var visualTree = TreeEditorUtility.GetVisualTree(nameof(UIElements.NodeDrawer));
            visualTree.CloneTree(this);

            var visualElement = this.Q("container");

            var nodeType = TypeLocater
                .GetAllTypes()
                .ThatInheritFrom<IBehaviourTreeNode>()
                .ThatHaveADefaultConstructor()
                .Where(type => type.InheritsFrom<RootNode>() == false);
            foreach (var type in nodeType) visualElement.Add(new DrawerNode(type));
        }
    }
}