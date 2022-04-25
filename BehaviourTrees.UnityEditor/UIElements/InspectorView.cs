using System;
using BehaviourTrees.Model;
using BehaviourTrees.UnityEditor.Data;
using UnityEditor;
using UnityEngine.UIElements;

namespace BehaviourTrees.UnityEditor.UIElements
{
    /// <summary>
    ///     Shows the field data set on the currently selected node and allows editing it.
    /// </summary>
    public class InspectorView : VisualElement
    {
        private readonly VisualElement _propertyViewContainer;
        private static BehaviourTreeEditor Window => BehaviourTreeEditor.GetOrOpen();
        private static EditorTreeContainer Tree => Window.TreeContainer;

        /// <summary>
        ///     Creates a new instance of the inspector view element.
        /// </summary>
        public InspectorView()
        {
            var visualTree = TreeEditorUtility.GetVisualTree(nameof(InspectorView));
            visualTree.CloneTree(this);

            _propertyViewContainer = this.Q("property-list");
        }

        /// <summary>
        ///     Sets the node the inspector is currently showing.
        /// </summary>
        /// <param name="nodeView">The node to show the data of.</param>
        public void SetToNode(NodeView nodeView)
        {
            _propertyViewContainer.Clear();
            if (nodeView == null) return;

            var node = nodeView.Node;

            foreach (var info in nodeView.Node.GetFillableFieldsFromType())
            {
                var splitName = TreeEditorUtility.SplitPascalCase(info.FieldName);
                var value = node.Properties[info.FieldName];
                var callback = new Action<object>(o =>
                {
                    Undo.RecordObject(Tree, $"Edit node properties: ({node.GetType().Name} -> {splitName})");
                    node.Properties[info.FieldName] = o;
                    Tree.MarkDirty();
                });

                var propertyView =
                    TreeEditorUtility.IsBlackboardField(node.RepresentingType, info.FieldName, out var blackboardType)
                        ? PropertyView.CreateBlackboardDropdown
                            (splitName, blackboardType, value as string, callback)
                        : PropertyView.CreateEditor(splitName, info.FieldType, value, callback);

                _propertyViewContainer.Add(propertyView);
            }
        }

        /// <summary>
        ///     Instantiates a <see cref="InspectorView" /> using the data read from a UXML file
        /// </summary>
        public new class UxmlFactory : UxmlFactory<InspectorView, UxmlTraits> { }
    }
}