using System;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTrees.UnityEditor
{
    public class EditorModelExtension
    {
        public Dictionary<string, Type> BlackboardKeys = new Dictionary<string, Type>();
        public Dictionary<string, Vector2> NodePositions = new Dictionary<string, Vector2>();

        public void InvokeBlackboardKeysChanged(object sender)
        {
            BlackboardKeysChanged?.Invoke(sender, EventArgs.Empty);
        }

        public event EventHandler BlackboardKeysChanged;
    }
}