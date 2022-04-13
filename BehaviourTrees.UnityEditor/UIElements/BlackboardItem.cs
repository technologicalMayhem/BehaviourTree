using System;
using UnityEngine.UIElements;

namespace BehaviourTrees.UnityEditor.UIElements
{
    /// <summary>
    ///     Used to show blackboard keys in the blackboard view.
    /// </summary>
    public class BlackboardItem : VisualElement
    {
        /// <summary>
        ///     Create a new blackboard item element.
        /// </summary>
        /// <param name="key">The text to show in the key field.</param>
        /// <param name="type">The text to show in the type field.</param>
        /// <param name="callback">The callback for when remove is clicked.</param>
        public BlackboardItem(string key, string type, Action callback)
        {
            var visualTree = TreeEditorUtility.GetVisualTree(nameof(BlackboardItem));
            visualTree.CloneTree(this);

            var keyLabel = this.Q<Label>("key");
            var typeLabel = this.Q<Label>("type");
            var removeLabel = this.Q<Label>("remove");

            keyLabel.text = key;
            typeLabel.text = type;
            removeLabel.AddManipulator(new Clickable(callback));
        }
    }
}