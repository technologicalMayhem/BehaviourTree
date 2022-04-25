using System;
using System.Collections.Generic;
using BehaviourTrees.Model;
using UnityEngine;

namespace BehaviourTrees.UnityEditor.Data
{
    /// <summary>
    ///     Contains data about the <see cref="BehaviourTreeEditor" /> that is only relevant to the editor.
    /// </summary>
    public class EditorModelExtension
    {
        /// <summary>
        ///     Contains a list of blackboard keys and their types.
        /// </summary>
        public readonly Dictionary<string, Type> BlackboardKeys = new Dictionary<string, Type>();

        /// <summary>
        ///     Contains the positions of <see cref="NodeModel" /> in the editor. The positions are stored by the id of the node.
        /// </summary>
        public readonly Dictionary<string, Vector2> NodePositions = new Dictionary<string, Vector2>();

        /// <summary>
        ///     Raises the <see cref="BlackboardKeysChanged" /> event.
        /// </summary>
        /// <param name="sender">The sender of this event.</param>
        public void InvokeBlackboardKeysChanged(object sender)
        {
            BlackboardKeysChanged?.Invoke(sender, EventArgs.Empty);
        }

        /// <summary>
        ///     Gets raised if changes are being made to keys in the blackboard.
        /// </summary>
        public event EventHandler BlackboardKeysChanged;
    }
}