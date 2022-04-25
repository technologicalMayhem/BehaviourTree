using System;
using System.Collections.Generic;
using System.Linq;
using BehaviourTrees.Model;
using BehaviourTrees.UnityEditor.Data;
using JetBrains.Annotations;

namespace BehaviourTrees.UnityEditor.Validation
{
    /// <summary>
    ///     Checks if there are problems with the blackboard.
    /// </summary>
    [UsedImplicitly]
    public class BlackboardValidator : TreeValidator
    {
        /// <inheritdoc />
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
                    Severity = ProblemSeverity.Error,
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
                    Severity = ProblemSeverity.Error,
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
                    Severity = ProblemSeverity.Error,
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
                Severity = ProblemSeverity.Warning,
                Message = $"The blackboard key {pair.Key} is unused."
            });
        }

        /// <summary>
        ///     Returns data about all the fields in model that represent blackboard keys.
        /// </summary>
        /// <param name="model">The model get fields from.</param>
        /// <param name="container">The container of the blackboard data.</param>
        /// <returns>The data about the fields used for blackboard keys.</returns>
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

        /// <summary>
        ///     Contains data about a field used store a key for the blackboard.
        /// </summary>
        private struct BlackboardFieldData
        {
            public string FieldName;
            public string BlackboardKey;
            public Type NodeExpectedType;
            public Type ActualBlackboardType;
        }
    }
}