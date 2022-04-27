using System;

namespace BehaviourTrees.UnityEditor.Interfaces
{
    /// <summary>
    ///     Provides events to be notified if this element has been selected or unselected.
    /// </summary>
    public interface ISelectionCallback
    {
        /// <summary>
        ///     Gets raised if this element has been selected.
        /// </summary>
        public event EventHandler Selected;

        /// <summary>
        ///     Gets raised if this element has been unselected.
        /// </summary>
        public event EventHandler Unselected;
    }
}