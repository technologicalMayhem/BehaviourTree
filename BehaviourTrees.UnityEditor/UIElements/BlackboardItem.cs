using System;
using UnityEngine.UIElements;

namespace BehaviourTrees.UnityEditor.UIElements
{
    public class BlackboardItem : VisualElement
    {
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