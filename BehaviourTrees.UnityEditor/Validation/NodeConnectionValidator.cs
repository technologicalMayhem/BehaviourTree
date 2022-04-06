using System.Collections.Generic;
using System.Linq;

namespace BehaviourTrees.UnityEditor.Validation
{
    public class NodeConnectionValidator : TreeValidator
    {
        public override IEnumerable<ValidationResult> Validate(EditorTreeContainer container)
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
                   Message = $"Connectionless node. Consider removing or connecting the node."
                });
        }
    }
}