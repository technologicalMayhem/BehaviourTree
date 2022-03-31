using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BehaviourTrees.Core.Blackboard;
using BehaviourTrees.Model;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviourTrees.UnityEditor.UIElements
{
    public class BlackboardView : VisualElement
    {
        private VisualElement _collection;

        public BehaviourTreeView TreeView
        {
            set
            {
                if (_treeView != null)
                {
                    _treeView.TreeLoaded -= Loaded;
                }

                _treeView = value;
                if (_treeView != null)
                {
                    _treeView.TreeLoaded += Loaded;
                }
            }
        }

        private BehaviourTreeView _treeView;

        public BlackboardView()
        {
            var visualTree =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    EditorUtilities.LocateUiDefinitionFile(nameof(BlackboardView)));
            visualTree.CloneTree(this);

            styleSheets.Add(EditorUtilities.GetStyleSheet());

            _collection = this.Q("blackboard");
        }

        private void Loaded()
        {
            _treeView.TreeContainer.TreeModel.TreeHasChanged += Update;
            Update(null, null);
        }

        private void Update(object sender, EventArgs eventArgs)
        {
            _collection.Clear();
            var blackboardData = GetData();
            CreateVisualElements(blackboardData);
        }

        private void CreateVisualElements(IEnumerable<BlackboardData> blackboardDataList)
        {
            var blackboardKeyVisual = EditorUtilities.LoadPartialUi(nameof(BlackboardView), "BlackboardKey");
            var subscribedNodeVisual = EditorUtilities.LoadPartialUi(nameof(BlackboardView), "SubscribedNode");

            var groupedData = blackboardDataList.GroupBy(data => data.Key).ToList();
            foreach (var data in groupedData)
            {
                blackboardKeyVisual.CloneTree(_collection);
                var blackboardKey = _collection.Children().Last();

                var valueKey = blackboardKey.Q<TextElement>("value-key");
                if (string.IsNullOrEmpty(data.Key))
                {
                    valueKey.AddToClassList("blackboard-warning");
                    valueKey.text = "< No key defined! >";
                }
                else
                {
                    valueKey.text = data.Key;
                }

                blackboardKey.Q<TextElement>("value-type").text = data.First().ValueType;
                var subscribedNodes = blackboardKey.Q("subscribed-nodes");

                foreach (var blackboardData in data)
                {
                    subscribedNodeVisual.CloneTree(subscribedNodes);
                    var blackboardNode = subscribedNodes.Children().Last();
                    var button = blackboardNode.Q<Button>("node-goto");
                    button.text = "Goto node";
                    var modelPosition = blackboardData.Model.Position;
                    var newPosition = new Vector3(-modelPosition.X, -modelPosition.Y);
                    var contentContainerLayout = _treeView.contentViewContainer.parent.contentRect;
                    newPosition.x += contentContainerLayout.width / 2;
                    newPosition.y += contentContainerLayout.height / 2;
                    button.clicked += () => _treeView.MoveTo(new Vector2(newPosition.x, newPosition.y));
                    blackboardNode.Q<TextElement>("node-type").text = blackboardData.Model.Type.Name;
                    blackboardNode.Q<TextElement>("node-access").text = blackboardData.Access;
                }
            }
        }

        private IEnumerable<BlackboardData> GetData()
        {
            var list = new List<BlackboardData>();

            foreach (var node in _treeView.TreeContainer.TreeModel.Nodes)
            {
                var collection = node.Type
                    .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                    .Where(info =>
                        info.GetCustomAttribute<BlackboardKeyAttribute>() != null
                        && info.FieldType.IsConstructedGenericType
                    ).Where(info =>
                        info.FieldType.GetGenericTypeDefinition() == typeof(Core.Blackboard.ISet<>)
                        || info.FieldType.GetGenericTypeDefinition() == typeof(IGet<>)
                        || info.FieldType.GetGenericTypeDefinition() == typeof(IGetSet<>)
                    ).Select(info =>
                    {
                        var key = info.GetCustomAttribute<BlackboardKeyAttribute>().Key;
                        var blackboardKey = LookupFieldNameValue(node, key);
                        var type = info.FieldType.GenericTypeArguments[0];
                        var genericTypeDefinition = info.FieldType.GetGenericTypeDefinition();
                        var accessorText = string.Empty;

                        if (genericTypeDefinition == typeof(IGetSet<>))
                        {
                            accessorText = "Get | Set";
                        }

                        if (genericTypeDefinition == typeof(IGet<>))
                        {
                            accessorText = "Get";
                        }

                        if (genericTypeDefinition == typeof(Core.Blackboard.ISet<>))
                        {
                            accessorText = "Set";
                        }

                        return new BlackboardData
                        {
                            Key = blackboardKey,
                            ValueType = type.Name,
                            Model = node,
                            Access = accessorText
                        };
                    });
                list.AddRange(collection);
            }

            return list;
        }


        [CanBeNull]
        private static string LookupFieldNameValue(NodeModel node, string fieldName)
        {
            node.Values.TryGetValue(fieldName, out var value);
            return value as string ?? fieldName;
        }

        private void RemoveChildren()
        {
            while (_collection.Children().Any())
            {
                _collection.Remove(_collection.Children().First());
            }
        }

        private struct BlackboardData
        {
            public string Key;
            public NodeModel Model;
            public string ValueType;
            public string Access;
        }

        public new class UxmlFactory : UxmlFactory<BlackboardView, UxmlTraits> { }
    }
}