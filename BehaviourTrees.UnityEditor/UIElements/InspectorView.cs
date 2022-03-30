using UnityEditor;
using UnityEngine.UIElements;

namespace BehaviourTrees.UnityEditor.UIElements
{
    public class InspectorView : VisualElement
    {
        private readonly VisualElement _propertyViewContainer;
        private NodeView _node;

        public BehaviourTreeView TreeView;

        public InspectorView()
        {
            var visualTree =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    EditorUtilities.LocateUiDefinitionFile(nameof(InspectorView)));
            visualTree.CloneTree(this);

            _propertyViewContainer = this.Q("property-list");
        }

        public void SetToNode(NodeView node)
        {
            _propertyViewContainer.Clear();

            _node = node;
            if (_node != null)
                foreach (var value in _node.Node.Values)
                {
                    var propertyView = new PropertyView();
                    var val = value.Value;

                    propertyView.CreateEditor(TreeView.TreeContainer, value.Key, val.GetType(),
                        o => { _node.Node.Values[value.Key] = o; });
                    propertyView.SetValue(val);
                    _propertyViewContainer.Add(propertyView);
                }
        }

        public new class UxmlFactory : UxmlFactory<InspectorView, UxmlTraits> { }
    }
}