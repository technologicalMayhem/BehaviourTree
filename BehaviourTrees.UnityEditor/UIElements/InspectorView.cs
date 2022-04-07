using System;
using System.Reflection;
using BehaviourTrees.Model;
using UnityEditor;
using UnityEngine.UIElements;

namespace BehaviourTrees.UnityEditor.UIElements
{
    public class InspectorView : VisualElement
    {
        private readonly VisualElement _propertyViewContainer;
        public BlackboardView BlackboardView;

        public EditorTreeContainer Tree;

        public InspectorView()
        {
            var visualTree =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    TreeEditorUtility.LocateUiDefinitionFile(nameof(InspectorView)));
            visualTree.CloneTree(this);

            _propertyViewContainer = this.Q("property-list");
        }

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

                var propertyView = TreeEditorUtility.IsBlackboardField(node.RepresentingType, info.FieldName, out var blackboardType)
                        ? PropertyView.CreateBlackboardDropdown
                            (Tree, splitName, blackboardType, value as string, callback, BlackboardView)
                        : PropertyView.CreateEditor(splitName, info.FieldType, value, callback);

                _propertyViewContainer.Add(propertyView);
            }
        }

        public new class UxmlFactory : UxmlFactory<InspectorView, UxmlTraits> { }
    }
}