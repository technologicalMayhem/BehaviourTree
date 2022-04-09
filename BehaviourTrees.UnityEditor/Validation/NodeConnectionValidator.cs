using System.Collections.Generic;
using System.Linq;

namespace BehaviourTrees.UnityEditor.Validation
{
    public class NodeConnectionValidator : TreeValidator
    {
        public override IEnumerable<ValidationResult> Validate(EditorTreeContainer container)
        {
            var results = new List<ValidationResult>();

            results.AddRange(CheckForLoneNodes(container));
            results.AddRange(CheckForConnectedToSelf(container));

            return results;
        }

        private static IEnumerable<ValidationResult> CheckForConnectedToSelf(EditorTreeContainer container)
        {
            return container.TreeModel.Connections
                .Where(pair => pair.Value.Contains(pair.Key))
                .Select(pair => new ValidationResult
                {
                    Severity = Severity.Warning,
                    NodeId = pair.Key,
                    Message = "Node is connected to itself."
                });
        }

        private static IEnumerable<ValidationResult> CheckForLoneNodes(EditorTreeContainer container)
        {
            var list = container.TreeModel.Connections
                .SelectMany(pair => pair.Value
                    .Append(pair.Key))
                .ToList();

            return container.TreeModel.Nodes
                .Where(model => !list.Contains(model.Id))
                .Select(model => new ValidationResult
                {
                    Severity = Severity.Warning,
                    NodeId = model.Id,
                    Message = "Connectionless node. Consider removing or connecting the node."
                });
        }
    }
}