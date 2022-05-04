using UnityEngine.UIElements;

namespace BehaviourTrees.UnityEditor.UIElements
{
    /// <summary>
    ///     <p>Provides methods for the <see cref="Sidebar" /> to interact with this element.</p>
    ///     <p>Classes inheriting this will be instantiated and placed in the sidebar.</p>
    /// </summary>
    public abstract class SidebarElement : VisualElement
    {
        /// <summary>
        ///     Indicates if this element should be shown by default.
        /// </summary>
        public virtual bool ShowByDefault => true;

        /// <summary>
        ///     The default position in the sidebar. Higher number means further up.
        /// </summary>
        public virtual int DefaultPosition => 0;

        /// <summary>
        ///     The user facing name for this element.
        /// </summary>
        public virtual string FriendlyName => TreeEditorUtility.SplitPascalCase(GetType().Name);

        /// <summary>
        ///     Gets called after the element collapsed in the sidebar.
        /// </summary>
        public virtual void OnCollapsed() { }

        /// <summary>
        ///     Gets called before the element expands in the sidebar.
        /// </summary>
        public virtual void OnExpand() { }
    }
}