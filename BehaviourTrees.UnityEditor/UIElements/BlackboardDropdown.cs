using System;
using System.Linq;
using System.Text.RegularExpressions;
using BehaviourTrees.Model;
using UnityEngine.UIElements;

namespace BehaviourTrees.UnityEditor.UIElements
{
    /// <summary>
    ///     A dropdown element to select a key from the blackboard.
    /// </summary>
    public sealed class BlackboardDropdown : DropdownField
    {
        /// <summary>
        ///     The type for the blackboard dropdown.
        /// </summary>
        private readonly Type _blackboardType;

        /// <summary>
        ///     All callback to invoke when a new value has been selected.
        /// </summary>
        private readonly Action<string> _callback;

        /// <summary>
        ///     A reference to the tree container contained in the main editor window.
        /// </summary>
        private static EditorTreeContainer Tree => BehaviourTreeEditor.GetOrOpen().TreeContainer;

        /// <summary>
        ///     Create a new instance of the blackboard key element.
        /// </summary>
        /// <param name="blackboardType">The type that the key should be of.</param>
        /// <param name="key">The key that is already selected. May be an empty string if nothing is chosen.</param>
        /// <param name="callback">A callback that is called when the value has changed. Has the new value as parameter.</param>
        public BlackboardDropdown(Type blackboardType, string key, Action<string> callback)
            : base(null, 0, FormatSelectedValueCallback, FormatListItemCallback)
        {
            _blackboardType = blackboardType;
            _callback = callback;

            UpdateChoices();
            this.RegisterValueChangedCallback(ValueChanged);
            SetValueWithoutNotify($"{key} ({TreeEditorUtility.GetTypeName(blackboardType)})");

            Tree.ModelExtension.BlackboardKeysChanged += (sender, args) => UpdateChoices();
        }

        /// <summary>
        ///     Gets called if the value has changed.
        /// </summary>
        /// <param name="evt">The event data about the changed value.</param>
        private void ValueChanged(ChangeEvent<string> evt)
        {
            var key = Regex.Match(evt.newValue, @"(.+) \(.+\)").Groups[1].Value;
            _callback.Invoke(key);
        }

        /// <summary>
        ///     Updates the list of choices with all keys from the blackboard that match the type.
        /// </summary>
        private void UpdateChoices()
        {
            choices = Tree.ModelExtension.BlackboardKeys
                .Where(pair => _blackboardType.InheritsFrom(pair.Value))
                .Select(pair => $"{pair.Key} ({TreeEditorUtility.GetTypeName(pair.Value)})")
                .ToList();
        }

        private static string FormatListItemCallback(string arg)
        {
            return arg;
        }

        private static string FormatSelectedValueCallback(string arg)
        {
            return arg;
        }
    }
}