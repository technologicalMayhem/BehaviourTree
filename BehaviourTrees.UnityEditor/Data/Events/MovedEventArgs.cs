using System;
using UnityEngine;

namespace BehaviourTrees.UnityEditor.Data.Events
{
    public class MovedEventArgs : EventArgs
    {
        public readonly Vector2 OldPosition;
        public readonly Vector2 NewPosition;

        public MovedEventArgs(Vector2 oldPosition, Vector2 newPosition)
        {
            OldPosition = oldPosition;
            NewPosition = newPosition;
        }
    }
}