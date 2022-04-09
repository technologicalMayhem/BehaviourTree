using System;
using System.Collections.Generic;
using System.Linq;
using BehaviourTrees.Model;

namespace BehaviourTrees.UnityEditor.Validation
{
    public class BlackboardValidator : TreeValidator
    {
        public override IEnumerable<ValidationResult> Validate(EditorTreeContainer container)
        {
            var results = new List<ValidationResult>();

            results.AddRange(CheckForNodeUsingWrongType(container));
            results.AddRange(CheckForNodesWithMissingKeys(container));
            results.AddRange(CheckForMissingKeys(container));
            results.AddRange(CheckForObsoleteKeys(container));

            return results;
        }

        //Checks if a blackboard key a node is using would return a incorrect type from the blackboard
        private static IEnumerable<ValidationResult> CheckForNodeUsingWrongType(EditorTreeContainer container)
        {
            var results = new List<ValidationResult>();

            foreach (var node in container.TreeModel.Nodes)
            {
                var blackboardFields = GetBlackboardFields(node, container)
                    .Where(data => data.ActualBlackboardType != null);

                var mismatches = blackboardFields.Where(data =>
                {
                    container.ModelExtension.BlackboardKeys.TryGetValue(data.BlackboardKey, out var type);
                    return data.NodeExpectedType != type;
                });

                var validationResults = mismatches.Select(data => new ValidationResult
                {
                    Severity = Severity.Error,
                    NodeId = node.Id,
                    Message =
                        $"{data.FieldName} expects type {data.NodeExpectedType} for key {data.FieldName} but is {data.ActualBlackboardType} in blackboard instead."
                });

                results.AddRange(validationResults);
            }

            return results;
        }

        //Checks if there are nodes that don't have blackboard keys set
        private static IEnumerable<ValidationResult> CheckForNodesWithMissingKeys(EditorTreeContainer container)
        {
            var results = new List<ValidationResult>();

            foreach (var node in container.TreeModel.Nodes)
            {
                var blackboardFields = GetBlackboardFields(node, container);

                var missingFields = blackboardFields.Where(data =>
                {
                    node.Properties.TryGetValue(data.FieldName, out var value);
                    return string.IsNullOrEmpty(value as string);
                });

                var validationResults = missingFields.Select(data => new ValidationResult
                {
                    Severity = Severity.Error,
                    NodeId = node.Id,
                    Message = $"The field {data.FieldName} does not have a key set."
                });

                results.AddRange(validationResults);
            }

            return results;
        }


        //Checks if there are keys that nodes are using that do not exist in the blackboard
        private static IEnumerable<ValidationResult> CheckForMissingKeys(EditorTreeContainer container)
        {
            var results = new List<ValidationResult>();

            foreach (var node in container.TreeModel.Nodes)
            {
                var unusedKeys = GetBlackboardFields(node, container)
                    .Where(data => !string.IsNullOrEmpty(data.BlackboardKey))
                    .Where(data => !container.ModelExtension.BlackboardKeys.ContainsKey(data.BlackboardKey));

                var validationResults = unusedKeys.Select(data => new ValidationResult
                {
                    Severity = Severity.Error,
                    NodeId = node.Id,
                    Message = $"The blackboard key {data.BlackboardKey} in field {data.FieldName} does not exist."
                });

                results.AddRange(validationResults);
            }

            return results;
        }

        //Checks if there are keys in the blackboard that no node is using
        private static IEnumerable<ValidationResult> CheckForObsoleteKeys(EditorTreeContainer container)
        {
            var obsoleteKeys = container.ModelExtension.BlackboardKeys
                .Where(pair => container.TreeModel.Nodes
                    .All(model => GetBlackboardFields(model, container)
                        .All(data => data.BlackboardKey != pair.Key))
                );

            return obsoleteKeys.Select(pair => new ValidationResult
            {
                Severity = Severity.Warning,
                Message = $"The blackboard key {pair.Key} is unused."
            });
        }

        private static IEnumerable<BlackboardFieldData> GetBlackboardFields(NodeModel model,
            EditorTreeContainer container)
        {
            return model.Properties.Where(pair =>
                    TreeEditorUtility.IsBlackboardField(model.RepresentingType, pair.Key, out _))
                .Select(pair =>
                {
                    TreeEditorUtility.IsBlackboardField(model.RepresentingType, pair.Key,
                        out var expectedBlackboardType);
                    Type actualBlackboardType = null;
                    if (pair.Value is string s)
                        container.ModelExtension.BlackboardKeys.TryGetValue(s, out actualBlackboardType);

                    return new BlackboardFieldData
                    {
                        FieldName = pair.Key,
                        BlackboardKey = pair.Value as string,
                        NodeExpectedType = expectedBlackboardType,
                        ActualBlackboardType = actualBlackboardType
                    };
                });
        }

        private struct BlackboardFieldData
        {
            public string FieldName;
            public string BlackboardKey;
            public Type NodeExpectedType;
            public Type ActualBlackboardType;
        }
    }
}