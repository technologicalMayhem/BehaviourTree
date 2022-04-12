using BehaviourTrees.UnityEditor.Validation;
using UnityEditor;
using UnityEngine.UIElements;

namespace BehaviourTrees.UnityEditor.UIElements
{
    public class ProblemItem : VisualElement
    {
        private ProblemItem()
        {
            var visualTree = TreeEditorUtility.GetVisualTree(nameof(ProblemItem));
            visualTree.CloneTree(this);

            styleSheets.Add(TreeEditorUtility.GetStyleSheet());
        }

        public static ProblemItem Create(ValidationResult result)
        {
            var problemItem = new ProblemItem();

            var severity = problemItem.Q<Label>("severity");
            severity.text = result.Severity.ToString();
            severity.AddToClassList(result.Severity == Severity.Error ? "problem-error" : "problem-warning");

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