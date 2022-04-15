using System;
using UnityEngine.UIElements;

namespace BehaviourTrees.UnityEditor.UIElements
{
    /// <inheritdoc />
    public class SplitView : TwoPaneSplitView
    {
        private readonly VisualElement _dragLineAnchor;

        /// <summary>
        ///     Creates a new instance of the SplitView element.
        /// </summary>
        public SplitView()
        {
            _dragLineAnchor = this.Q("unity-dragline-anchor");
        }

        /// <summary>
        ///     Registers a callback that gets called when the dragline has been moved and the layout recalculated.
        /// </summary>
        /// <param name="callback">The callback to use.</param>
        public void RegisterDraglineMovedCallback(Action<GeometryChangedEvent> callback)
        {
            _dragLineAnchor.RegisterCallback(new EventCallback<GeometryChangedEvent>(callback));
        }

        /// <summary>
        ///     The position of the dragline.
        /// </summary>
        public float DragLinePosition
        {
            get => GetDragLineOffset();
            set
            {
                if (fixedPane == null) return;
                SetFixedPaneDimension(value);
                SetDragLineOffset(value);
            }
        }

        private void SetDragLineOffset(float offset)
        {
            if (orientation == TwoPaneSplitViewOrientation.Horizontal)
                _dragLineAnchor.style.left = offset;
            else
                _dragLineAnchor.style.top = offset;
        }

        private float GetDragLineOffset()
        {
            return orientation == TwoPaneSplitViewOrientation.Horizontal
                ? _dragLineAnchor.style.left.value.value
                : _dragLineAnchor.style.top.value.value;
        }

        private void SetFixedPaneDimension(float dimension)
        {
            if (orientation == TwoPaneSplitViewOrientation.Horizontal)
                fixedPane.style.width = dimension;
            else
                fixedPane.style.height = dimension;
        }

        /// <summary>
        ///     Instantiates a <see cref="SplitView" /> using the data read from a UXML file
        /// </summary>
        public new class UxmlFactory : UxmlFactory<SplitView, UxmlTraits> { }
    }
}