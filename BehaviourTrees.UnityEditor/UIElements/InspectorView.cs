using System;
using BehaviourTrees.UnityEditor.Data;
using BehaviourTrees.UnityEditor.Inspector;
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
        private static EditorTreeContainer Container => Window.TreeContainer;

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
        ///     Assigns the object the editor is currently showing. Pass null to clear window.
        /// </summary>
        /// <param name="editable">The object to edit the data of.</param>
        public void AssignObject(IEditable editable)
        {
            _propertyViewContainer.Clear();
            if (editable == null) return;

            foreach (var property in editable.GetProperties())
            {
                var callback = new Action<object>(o =>
                {
                    Undo.RecordObject(Container, $"Edit properties: {property.Name}");
                    editable.SetValue(property.Name, o);
                    Container.MarkDirty();
                });

                if (editable is NodeView view && TreeEditorUtility.IsBlackboardField
                        (view.Node.RepresentingType, property.Name, out var blackboardType))
                    _propertyViewContainer.Add(PropertyView.CreateBlackboardDropdown
                        (property.FriendlyName, blackboardType, property.Value as string, callback));
                else
                    _propertyViewContainer.Add(
                        PropertyView.CreateEditor(property.FriendlyName, property.Type, property.Value, callback));
            }
        }

        /// <summary>
        ///     Instantiates a <see cref="InspectorView" /> using the data read from a UXML file
        /// </summary>
        public new class UxmlFactory : UxmlFactory<InspectorView, UxmlTraits> { }
    }
}