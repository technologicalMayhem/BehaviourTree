using System;
using System.Linq;
using System.Text.RegularExpressions;
using BehaviourTrees.Model;
using UnityEngine.UIElements;

namespace BehaviourTrees.UnityEditor.UIElements
{
    public sealed class BlackboardDropdown : DropdownField
    {
        private readonly Type _blackboardType;
        private readonly Action<string> _callback;
        private static EditorTreeContainer Tree => BehaviourTreeEditor.GetOrOpen().TreeContainer;

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

        private void ValueChanged(ChangeEvent<string> evt)
        {
            var key = Regex.Match(evt.newValue, @"(.+) \(.+\)").Groups[1].Value;
            _callback.Invoke(key);
        }

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