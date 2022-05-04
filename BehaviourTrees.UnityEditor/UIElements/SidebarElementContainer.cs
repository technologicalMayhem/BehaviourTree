using UnityEngine.UIElements;

namespace BehaviourTrees.UnityEditor.UIElements
{
    /// <summary>
    ///     Provides a header and the ability to be collapsed for a <see cref="SidebarElement" />.
    /// </summary>
    public class SidebarElementContainer : VisualElement
    {
        private const string CollapsedChar = "▶";
        private const string ExpandedChar = "▼";

        private readonly VisualElement _contentHolder;
        private readonly SidebarElement _content;
        private readonly Label _collapseButton;

        private bool _isCollapsed;

        /// <summary>
        ///     Contains a element in the sidebar and allows it to be collapsed.
        /// </summary>
        /// <param name="element">The element this element should wrap around.</param>
        public SidebarElementContainer(SidebarElement element)
        {
            var visualTree = TreeEditorUtility.GetVisualTree(nameof(SidebarElementContainer));
            visualTree.CloneTree(this);
            _content = element;

            _contentHolder = this.Q("sidebar-content");
            _collapseButton = this.Q<Label>("element-collapse");
            var header = this.Q("sidebar-header");
            var elementName = this.Q<Label>("element-name");

            _contentHolder.Add(element);
            elementName.text = _content.FriendlyName;
            header.AddManipulator(new Clickable(ToggleCollapseState));
            _collapseButton.text = ExpandedChar;
        }

        private void ToggleCollapseState()
        {
            if (_isCollapsed)
                Expand();
            else
                Collapse();

            _isCollapsed = !_isCollapsed;
        }

        private void Collapse()
        {
            _collapseButton.text = CollapsedChar;
            _contentHolder.AddToClassList("hide");

            _content.OnCollapsed();
        }

        private void Expand()
        {
            _content.OnExpand();

            _collapseButton.text = ExpandedChar;
            _contentHolder.RemoveFromClassList("hide");
        }
    }
}