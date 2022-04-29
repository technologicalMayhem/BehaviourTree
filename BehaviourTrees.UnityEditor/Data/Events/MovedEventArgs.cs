using System;
using BehaviourTrees.UnityEditor.UIElements;
using UnityEngine;

namespace BehaviourTrees.UnityEditor.Data.Events
{
    /// <summary>
    ///     Provides data about a <see cref="NodeView" /> object having moved.
    /// </summary>
    public class MovedEventArgs : EventArgs
    {
        /// <summary>
        ///     The old position of the <see cref="NodeView" />.
        /// </summary>
        public readonly Vector2 OldPosition;

        /// <summary>
        ///     The new position of the <see cref="NodeView" />.
        /// </summary>
        public readonly Vector2 NewPosition;

        /// <summary>
        ///     Creates a new instance of <see cref="MovedEventArgs" />.
        /// </summary>
        /// <param name="oldPosition">The old position of the <see cref="NodeView" />.</param>
        /// <param name="newPosition">The new position of the <see cref="NodeView" />.</param>
        public MovedEventArgs(Vector2 oldPosition, Vector2 newPosition)
        {
            OldPosition = oldPosition;
            NewPosition = newPosition;
        }
    }
}