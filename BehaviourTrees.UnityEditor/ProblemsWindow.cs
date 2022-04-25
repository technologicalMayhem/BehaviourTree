using System;
using System.Collections.Generic;
using System.Linq;
using BehaviourTrees.Model;
using BehaviourTrees.UnityEditor.Data;
using BehaviourTrees.UnityEditor.UIElements;
using BehaviourTrees.UnityEditor.Validation;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviourTrees.UnityEditor
{
    /// <summary>
    ///     This window shows problems that are detected whilst the behaviour tree is open in the
    ///     <see cref="BehaviourTreeEditor" />.
    /// </summary>
    [ExecuteAlways]
    public class ProblemsWindow : EditorWindow
    {
        /// <summary>
        ///     A collection of all <see cref="TreeValidator" />s found in the current app domain.
        /// </summary>
        private static TreeValidator[] _validators;

        /// <summary>
        ///     A visual element in which a list of <see cref="ProblemItem" />s is placed to be displayed to the user.
        /// </summary>
        private VisualElement _problemsList;

        /// <summary>
        ///     Indicates whether should be open or close.
        /// </summary>
        private static bool ShouldBeOpen =>
            HasOpenInstances<BehaviourTreeEditor>() && BehaviourTreeEditor.GetOrOpen().TreeContainer != null;

        /// <summary>
        ///     A reference to the tree container contained in the main editor window.
        /// </summary>
        private static EditorTreeContainer Container => BehaviourTreeEditor.GetOrOpen().TreeContainer;

        private void Update()
        {
            if (!ShouldBeOpen) Close();
        }

        /// <summary>
        ///     <para>Gets called when the window opens.</para>
        ///     <para>Populates the window with UI elements and sets them all up.</para>
        /// </summary>
        [UsedImplicitly]
        private void CreateGUI()
        {
            var root = rootVisualElement;

            // Import UXML
            var visualTree = TreeEditorUtility.GetVisualTree(nameof(ProblemsWindow));
            visualTree.CloneTree(root);

            //Get style sheet
            var styleSheet = TreeEditorUtility.GetStyleSheet();
            root.styleSheets.Add(styleSheet);

            root.Q<Button>("analyze").clicked += Analyze;
            _problemsList = root.Q("problems");

            Analyze();
        }

        /// <summary>
        ///     Opens the problem window.
        /// </summary>
        public static void OpenWindow()
        {
            if (!ShouldBeOpen) return;
            _validators ??= GetValidators();
            var window = GetWindow<ProblemsWindow>();
            window.titleContent = new GUIContent("Problems");
            window.ShowUtility();
        }

        /// <summary>
        ///     Finds all classes implementing <see cref="TreeValidator" /> and returns them.
        /// </summary>
        private static TreeValidator[] GetValidators()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes()
                    .Where(type => type.InheritsFrom<TreeValidator>() && !type.IsAbstract))
                .Select(type => Activator.CreateInstance(type) as TreeValidator)
                .ToArray();
        }

        /// <summary>
        ///     Analyzes the <see cref="EditorTreeContainer" /> currently loaded in the editor and places all problems
        ///     found in <see cref="_problemsList" />.
        /// </summary>
        private void Analyze()
        {
            _problemsList.Clear();
            var problems = new List<ValidationResult>();

            foreach (var validator in _validators) problems.AddRange(validator.Validate(Container));

            foreach (var problem in problems) _problemsList.Add(ProblemItem.Create(problem));
        }
    }
}