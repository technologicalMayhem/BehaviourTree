using System;
using System.Linq;
using System.Text.RegularExpressions;
using BehaviourTrees.Model;
using UnityEngine.UIElements;

namespace BehaviourTrees.UnityEditor.UIElements
{
    public sealed class BlackboardDropdown : DropdownField
    {
        private readonly EditorTreeContainer _tree;
        private readonly Type _blackboardType;
        private readonly Action<string> _callback;

        public BlackboardDropdown(EditorTreeContainer tree, Type blackboardType, string key, Action<string> callback)
            : base(null, 0, FormatSelectedValueCallback, FormatListItemCallback)
        {
            _tree = tree;
            _blackboardType = blackboardType;
            _callback = callback;
            
            UpdateChoices();
            this.RegisterValueChangedCallback(ValueChanged);
            SetValueWithoutNotify($"{key} ({TreeEditorUtility.GetTypeName(blackboardType)})");
            
            _tree.ModelExtension.BlackboardKeysChanged += (sender, args) => UpdateChoices();
        }

        private void ValueChanged(ChangeEvent<string> evt)
        {
            var key = Regex.Match(evt.newValue, @"(.+) \(.+\)").Groups[1].Value;
            _callback.Invoke(key);
        }

        private void UpdateChoices()
        {
            choices = _tree.ModelExtension.BlackboardKeys
                .Where(pair => _blackboardType.InheritsFrom(pair.Value))
                .Select(pair => $"{pair.Key} ({TreeEditorUtility.GetTypeName(pair.Value)})")
                .ToList();
        }

        private static string FormatListItemCallback(string arg) => arg;

        private static string FormatSelectedValueCallback(string arg) => arg;
    }
}