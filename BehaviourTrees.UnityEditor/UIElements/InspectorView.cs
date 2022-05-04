using System;
using System.Collections.Generic;
using System.Linq;
using BehaviourTrees.UnityEditor.Data;
using BehaviourTrees.UnityEditor.Inspector;
using UnityEditor;
using UnityEngine.UIElements;

namespace BehaviourTrees.UnityEditor.UIElements
{
    /// <summary>
    ///     Shows the field data set on the currently selected node and allows editing it.
    /// </summary>
    public class InspectorView : SidebarElement
    {
        /// <inheritdoc />
        public override int DefaultPosition => 100;
        private readonly VisualElement _propertyViewContainer;
        private readonly VisualElement _notSelected;
        private readonly VisualTreeAsset _inspectorSeparator;
        private static BehaviourTreeEditor Window => BehaviourTreeEditor.GetOrOpen();
        private static EditorTreeContainer Container => Window.TreeContainer;

        /// <summary>
        ///     Creates a new instance of the inspector view element.
        /// </summary>
        public InspectorView()
        {
            var visualTree = TreeEditorUtility.GetVisualTree(nameof(InspectorView));
            visualTree.CloneTree(this);
            _inspectorSeparator = TreeEditorUtility.GetVisualTree("InspectorSeparator");

            _propertyViewContainer = this.Q("property-list");
            _notSelected = this.Q("not-selected");
        }

        /// <summary>
        ///     Assigns the object the editor is currently showing. Pass null to clear window.
        /// </summary>
        /// <param name="editable">The object to edit the data of.</param>
        public void AssignObject(IEditable editable)
        {
            _propertyViewContainer.Clear();
            if (editable == null)
            {
                _notSelected.RemoveFromClassList("hide");
                _propertyViewContainer.AddToClassList("hide");
                return;
            }

            _propertyViewContainer.RemoveFromClassList("hide");
            _notSelected.AddToClassList("hide");

            //Get properties and sort them into a dictionary.
            //Properties are grouped by category name and the categories are ordered by priority first, then category.
            var propertyInfos = editable.GetProperties().ToArray();
            var dictionary = new Dictionary<string, IEnumerable<PropertyInfo>>(propertyInfos
                .Select(info => info.CategoryName)
                .Distinct()
                .Select(category => new KeyValuePair<string, IEnumerable<PropertyInfo>>(
                    string.IsNullOrWhiteSpace(category) ? string.Empty : category,
                    propertyInfos.Where(info => info.CategoryName == category)))
                .OrderByDescending(pair => propertyInfos.First(info => info.CategoryName == pair.Key).CategoryOrder)
                .ThenBy(pair => pair.Key)
            );
            //Add properties to the inspector window by category
            foreach (var (category, list) in dictionary)
            {
                //Do not add a category header if the category name is empty
                if (category != string.Empty)
                {
                    var separator = new VisualElement();
                    _inspectorSeparator.CloneTree(separator);
                    separator.Q<Label>("separator-text").text = category;
                    _propertyViewContainer.Add(separator);
                }

                //Add properties
                foreach (var property in list)
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
        }

        /// <summary>
        ///     Instantiates a <see cref="InspectorView" /> using the data read from a UXML file
        /// </summary>
        public new class UxmlFactory : UxmlFactory<InspectorView, UxmlTraits> { }
    }
}