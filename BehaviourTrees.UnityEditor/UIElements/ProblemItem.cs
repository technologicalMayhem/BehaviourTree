using BehaviourTrees.UnityEditor.Validation;
using UnityEditor;
using UnityEngine.UIElements;

namespace BehaviourTrees.UnityEditor.UIElements
{
    /// <summary>
    ///     A visual element to represent a problem in the problems window.
    /// </summary>
    public class ProblemItem : VisualElement
    {
        /// <summary>
        ///     Creates a new instance of the ProblemItem element.
        /// </summary>
        private ProblemItem()
        {
            var visualTree = TreeEditorUtility.GetVisualTree(nameof(ProblemItem));
            visualTree.CloneTree(this);

            styleSheets.Add(TreeEditorUtility.GetStyleSheet());
        }

        /// <summary>
        ///     Creates a new ProblemItem for the <see cref="ValidationResult" />.
        /// </summary>
        /// <param name="result">The validation result to create a ProblemItem from.</param>
        /// <returns>The newly created ProblemItem.</returns>
        public static ProblemItem Create(ValidationResult result)
        {
            var problemItem = new ProblemItem();

            var severity = problemItem.Q<Label>("severity");
            severity.text = result.Severity.ToString();
            severity.AddToClassList(result.Severity == ProblemSeverity.Error ? "problem-error" : "problem-warning");

            var jump = problemItem.Q<Label>("jump");
            jump.text = result.NodeId;
            jump.AddManipulator(new Clickable(() =>
            {
                EditorWindow.GetWindow<BehaviourTreeEditor>().TreeView.MoveTo(result.NodeId);
            }));

            problemItem.Q<Label>("message").text = result.Message;

            return problemItem;
        }
    }
}