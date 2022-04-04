using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviourTrees.UnityEditor.UIElements
{
    public class BlackboardView : VisualElement
    {
        public EditorTreeContainer Container
        {
            get => _container;
            set
            {
                _container = value;
                UpdateBlackboard();
            }
        }

        private EditorTreeContainer _container;
        private readonly VisualElement _list;
        private readonly VisualElement _errors;
        private readonly TextField _newKey;
        private readonly TextField _newTypeSearch;
        private readonly DropdownField _newTypeList;
        private Type[] _choices;
        private readonly Type[] _nonGenericTypes;
        private readonly Type[] _genericTypes;

        private const string MatchGeneric = @"(\b[^<>]+)\<(.+)\>$";
        private const string SelectParameters = @"(\b[^,]+)";

        public BlackboardView()
        {
            var visualTree =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    TreeEditorUtility.LocateUiDefinitionFile(nameof(BlackboardView)));
            visualTree.CloneTree(this);

            //Gets all types that could be created in the blackboard
            var allTypes = AppDomain.CurrentDomain.GetAssemblies()
                .Where(assembly => !assembly.FullName.StartsWith("UnityEditor"))
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => !type.IsAbstract || !type.IsInterface)
                .ToArray();

            _nonGenericTypes = allTypes.Where(type => !type.IsGenericType).ToArray();
            _genericTypes = allTypes.Where(type => type.IsGenericType).ToArray();

            _newKey = this.Q<TextField>("new-key");
            _newTypeSearch = this.Q<TextField>("new-type-search");
            _newTypeList = this.Q<DropdownField>("new-type-list");
            _errors = this.Q("new-error");
            var button = this.Q<Button>("new-create");
            _list = this.Q("blackboard-list");

            _newTypeSearch.RegisterValueChangedCallback(UpdateList);
            button.clicked += CreateNewKey;
        }

        private void UpdateList(ChangeEvent<string> evt)
        {
            var match = Regex.Match(evt.newValue, MatchGeneric);
            var matchingTypes = Array.Empty<Type>();
            if (match.Success)
            {
                var parameterCapture = match.Groups[2].Value;
                var parameters = new List<Type>();

                for (var parameterMatch = Regex.Match(parameterCapture, SelectParameters);
                     parameterMatch.Success;
                     parameterMatch = parameterMatch.NextMatch())
                {
                    parameters.Add(_nonGenericTypes.FirstOrDefault(type => type.Name == parameterMatch.Value));
                }

                var genericBase = _genericTypes
                    .FirstOrDefault(type => type.Name == match.Groups[1].Value + $"`{parameters.Count}");

                if (genericBase != null && parameters.All(type => type != null))
                {
                    var generic = genericBase.MakeGenericType(parameters.ToArray());
                    matchingTypes = new[] { generic };
                }
            }

            if (matchingTypes.Length == 0)
            {
                matchingTypes = _nonGenericTypes
                    .Where(type => TreeEditorUtility.GetTypeName(type).Contains(evt.newValue)).ToArray();
            }

            _choices = matchingTypes.Take(50).ToArray();
            _newTypeList.choices = _choices.Select(TreeEditorUtility.GetTypeName).ToList();
            _newTypeList.SetValueWithoutNotify(_newTypeList.choices.FirstOrDefault());
        }

        private void CreateNewKey()
        {
            if (CheckForErrors()) return;

            var type = _choices.First(t => TreeEditorUtility.GetTypeName(t) == _newTypeList.value);
            _container.ModelExtension.BlackboardKeys[_newKey.value] = type;
            _container.ModelExtension.InvokeBlackboardKeysChanged(this);

            UpdateBlackboard();
        }

        private bool CheckForErrors()
        {
            _errors.Clear();
            var errors = new List<string>();

            if (string.IsNullOrEmpty(_newKey.value))
                errors.Add("No key name set.");

            if (_container.ModelExtension.BlackboardKeys.ContainsKey(_newKey.value))
                errors.Add("Key already exists.");

            if (_choices.All(type => TreeEditorUtility.GetTypeName(type) != _newTypeList.value))
                errors.Add($"No type selected.");

            foreach (var error in errors)
                _errors.Add(new Label(error));

            return errors.Any();
        }

        private void UpdateBlackboard()
        {
            _list.Clear();

            foreach (var pair in _container.ModelExtension.BlackboardKeys)
            {
                var blackboardItem =
                    new BlackboardItem(pair.Key, TreeEditorUtility.GetTypeName(pair.Value), () => DeleteKey(pair.Key));
                _list.Add(blackboardItem);
            }
        }

        private void DeleteKey(string key)
        {
            _container.ModelExtension.BlackboardKeys.Remove(key);
            UpdateBlackboard();
        }

        public class UxmlFactory : UxmlFactory<BlackboardView, UxmlTraits> { }
    }
}