using UnityEngine.UIElements;

namespace BehaviourTrees.UnityEditor.UIElements
{
    /// <inheritdoc />
    public class SplitView : TwoPaneSplitView
    {
        /// <summary>
        ///     Instantiates a <see cref="SplitView" /> using the data read from a UXML file
        /// </summary>
        public new class UxmlFactory : UxmlFactory<SplitView, UxmlTraits> { }
    }
}