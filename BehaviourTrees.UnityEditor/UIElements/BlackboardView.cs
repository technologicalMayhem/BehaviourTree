using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace BehaviourTrees.UnityEditor.UIElements
{
    /// <summary>
    ///     Visualizes the blackboard and allows making edits to it.
    /// </summary>
    public class BlackboardView : VisualElement
    {
        /// <summary>
        ///     A regex for checking if a string is a descriptor for a generic class with type arguments.
        ///     Selects the generic class and type arguments as groups.
        /// </summary>
        private const string MatchGeneric = @"(\b[^<>]+)\<(.+)\>$";

        /// <summary>
        ///     Splits the type arguments.
        /// </summary>
        private const string SelectParameters = @"(\b[^,]+)";

        /// <summary>
        ///     Contains error text if an new cannot be created.
        /// </summary>
        private readonly VisualElement _errors;

        /// <summary>
        ///     A list of all generic types loaded in the current app domain.
        /// </summary>
        private readonly Type[] _genericTypes;

        /// <summary>
        ///     A visual elements containing blackboard keys to display.
        /// </summary>
        private readonly VisualElement _blackboardKeys;

        /// <summary>
        ///     The text field used by the user to enter a name for a new key to create.
        /// </summary>
        private readonly TextField _newKey;

        /// <summary>
        ///     The dropdown
        /// </summary>
        private readonly DropdownField _newTypeList;

        /// <summary>
        ///     A list of all non-generic, concrete types in the current app domain
        /// </summary>
        private readonly Type[] _nonGenericTypes;

        /// <summary>
        ///     The list of type choice for the currently given search string.
        /// </summary>
        private Type[] _choices;

        /// <summary>
        ///     A reference to the tree container contained in the main editor window.
        /// </summary>
        private static EditorTreeContainer Container => BehaviourTreeEditor.GetOrOpen().TreeContainer;

        /// <summary>
        ///     Creates a new blackboard view element.
        /// </summary>
        public BlackboardView()
        {
            var visualTree = TreeEditorUtility.GetVisualTree(nameof(BlackboardView));
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
            var newTypeSearch = this.Q<TextField>("new-type-search");
            _newTypeList = this.Q<DropdownField>("new-type-list");
            _errors = this.Q("new-error");
            var button = this.Q<Button>("new-create");
            _blackboardKeys = this.Q("blackboard-list");

            newTypeSearch.RegisterValueChangedCallback(UpdateList);
            button.clicked += CreateNewKey;
        }

        /// <summary>
        ///     Updates the list of results when searching for types.
        /// </summary>
        /// <param name="evt"></param>
        private void UpdateList(ChangeEvent<string> evt)
        {
            //Todo: Maybe make this a bit simpler? Looks hard to parse what's going on here
            var newChoices = new List<Type>();
            var genericType = TryConstructGeneric(evt.newValue);
            if (genericType != null)
                newChoices.Add(genericType);
            else
                newChoices.AddRange(_nonGenericTypes
                    .Where(type => TreeEditorUtility.GetTypeName(type).ToLower().Contains(evt.newValue.ToLower())));

            _choices = newChoices.Take(50).ToArray();
            _newTypeList.choices = _choices.Select(TreeEditorUtility.GetTypeName).ToList();
            var findExact = _choices.FirstOrDefault(type =>
                string.Equals(type.Name, evt.newValue, StringComparison.CurrentCultureIgnoreCase))?.Name;
            _newTypeList.SetValueWithoutNotify(findExact ?? _newTypeList.choices.FirstOrDefault());
        }

        /// <summary>
        ///     Attempts to constructs a generic type from the given string.
        /// </summary>
        /// <param name="typeString">
        ///     The string to create the type from. It has be in a format like this:
        ///     <c>List&lt;string&gt;</c> or <c>Dictionary&lt;string,string&gt;</c>
        /// </param>
        /// <returns>The constructed type or null.</returns>
        [CanBeNull]
        private Type TryConstructGeneric(string typeString)
        {
            var match = Regex.Match(typeString, MatchGeneric);
            if (match.Success)
            {
                //Get get type parameters
                var parameterCapture = match.Groups[2].Value;
                var parameters = new List<Type>();

                //Match generic type parameter and iterate trough them.
                for (var parameterMatch = Regex.Match(parameterCapture, SelectParameters);
                     parameterMatch.Success;
                     parameterMatch = parameterMatch.NextMatch())
                    parameters.Add(_nonGenericTypes.FirstOrDefault(type =>
                        string.Equals(type.Name, parameterMatch.Value, StringComparison.CurrentCultureIgnoreCase)));

                //Get generic class
                var genericBase = _genericTypes
                    .FirstOrDefault(type => string.Equals(type.Name, match.Groups[1].Value + $"`{parameters.Count}",
                        StringComparison.CurrentCultureIgnoreCase));

                //Create constructed generic class
                if (genericBase != null && parameters.All(type => type != null))
                    return genericBase.MakeGenericType(parameters.ToArray());
            }

            return null;
        }

        /// <summary>
        ///     Attempts to create a new key on the blackboard.
        /// </summary>
        private void CreateNewKey()
        {
            if (CheckForErrors()) return;

            var type = _choices.First(t => TreeEditorUtility.GetTypeName(t) == _newTypeList.value);
            Container.ModelExtension.BlackboardKeys[_newKey.value] = type;
            Container.ModelExtension.InvokeBlackboardKeysChanged(this);

            UpdateBlackboard();
        }

        /// <summary>
        ///     Checks if theres problems with the key the user is attempting to create and create text for it.
        /// </summary>
        /// <returns>Returns true if there are errors.</returns>
        private bool CheckForErrors()
        {
            _errors.Clear();
            var errors = new List<string>();

            if (string.IsNullOrEmpty(_newKey.value))
                errors.Add("No key name set.");

            if (Container.ModelExtension.BlackboardKeys.ContainsKey(_newKey.value))
                errors.Add("Key already exists.");

            if (_choices.All(type => TreeEditorUtility.GetTypeName(type) != _newTypeList.value))
                errors.Add("No type selected.");

            foreach (var error in errors)
                _errors.Add(new Label(error));

            return errors.Any();
        }

        /// <summary>
        ///     Updates the blackboard list with information form the model extension.
        /// </summary>
        public void UpdateBlackboard()
        {
            _blackboardKeys.Clear();

            foreach (var pair in Container.ModelExtension.BlackboardKeys)
            {
                var blackboardItem =
                    new BlackboardItem(pair.Key, TreeEditorUtility.GetTypeName(pair.Value), () => DeleteKey(pair.Key));
                _blackboardKeys.Add(blackboardItem);
            }
        }

        /// <summary>
        ///     Deletes a key from the blackboard.
        /// </summary>
        /// <param name="key">The name of the key to remove.</param>
        private void DeleteKey(string key)
        {
            Container.ModelExtension.BlackboardKeys.Remove(key);
            UpdateBlackboard();
        }

        /// <summary>
        ///     Instantiates a <see cref="BlackboardView" /> using the data read from a UXML file.
        /// </summary>
        public class UxmlFactory : UxmlFactory<BlackboardView, UxmlTraits> { }
    }
}