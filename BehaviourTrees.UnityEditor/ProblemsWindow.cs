using System;
using System.Collections.Generic;
using System.Linq;
using BehaviourTrees.Model;
using BehaviourTrees.UnityEditor.UIElements;
using BehaviourTrees.UnityEditor.Validation;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviourTrees.UnityEditor
{
    [ExecuteAlways]
    public class ProblemsWindow : EditorWindow
    {
        private static TreeValidator[] Validators;
        private VisualElement _problemsList;

        private static bool ShouldBeOpen =>
            HasOpenInstances<BehaviourTreeEditor>() && BehaviourTreeEditor.GetOrOpen().TreeContainer != null;

        private static EditorTreeContainer Container => BehaviourTreeEditor.GetOrOpen().TreeContainer;

        private void Update()
        {
            if (!ShouldBeOpen) Close();
        }

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

        public static void OpenWindow()
        {
            if (!ShouldBeOpen) return;
            if (Validators == null) GetValidators();
            var window = GetWindow<ProblemsWindow>();
            window.titleContent = new GUIContent("Problems");
            window.ShowUtility();
        }

        private static void GetValidators()
        {
            Validators = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes()
                    .Where(type => type.InheritsFrom<TreeValidator>() && !type.IsAbstract))
                .Select(type => Activator.CreateInstance(type) as TreeValidator)
                .ToArray();
        }

        private void Analyze()
        {
            _problemsList.Clear();
            var problems = new List<ValidationResult>();

            foreach (var validator in Validators) problems.AddRange(validator.Validate(Container));

            foreach (var problem in problems) _problemsList.Add(ProblemItem.Create(problem));
        }
    }
}