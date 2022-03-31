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
            var blackboardData = CollectBlackboardDataFromNodes();
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
                    
                    var modelPosition = blackboardData.Model.Position;
                    var newPosition = new Vector3(-modelPosition.X, -modelPosition.Y);
                    var contentContainerLayout = _treeView.contentViewContainer.parent.contentRect;
                    newPosition.x += contentContainerLayout.width / 2;
                    newPosition.y += contentContainerLayout.height / 2;
                    
                    var button = blackboardNode.Q<Button>("node-goto");
                    button.text = "Goto node";
                    button.clicked += () => _treeView.MoveTo(new Vector2(newPosition.x, newPosition.y));

                    blackboardNode.Q<TextElement>("node-type").text = blackboardData.Model.Type.Name;
                    blackboardNode.Q<TextElement>("node-access").text = blackboardData.Access;
                }
            }
        }

        private IEnumerable<BlackboardData> CollectBlackboardDataFromNodes()
        {
            var dataList = new List<BlackboardData>();
            const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            var fields = new Dictionary<NodeModel, IEnumerable<FieldInfo>>();

            foreach (var modelNode in _treeView.TreeContainer.TreeModel.Nodes)
            {
                var fieldInfos = modelNode.Type.GetFields(bindingFlags).Where(info =>
                    info.GetCustomAttribute<BlackboardKeyAttribute>() != null
                    && IsBlackboardAccessor(info.FieldType)
                );
                fields[modelNode] = fieldInfos;
            }

            foreach (var pair in fields)
            {
                var node = pair.Key;
                foreach (var info in pair.Value)
                {
                    var key = info.GetCustomAttribute<BlackboardKeyAttribute>().Key;
                    var blackboardKey = LookupModelValue(node, key);
                    
                    var type = info.FieldType.GenericTypeArguments[0];
                    var genericTypeDefinition = info.FieldType.GetGenericTypeDefinition();
                    var accessorText = GetAccessorText(genericTypeDefinition);

                    dataList.Add(new BlackboardData
                    {
                        Key = blackboardKey,
                        ValueType = type.Name,
                        Model = node,
                        Access = accessorText
                    });
                }
            }

            return dataList;
        }

        private static string GetAccessorText(Type genericTypeDefinition)
        {
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

            return accessorText;
        }


        private static bool IsBlackboardAccessor(Type type)
        {
            return type.IsConstructedGenericType
                   && type.GetGenericTypeDefinition() == typeof(Core.Blackboard.ISet<>)
                   || type.GetGenericTypeDefinition() == typeof(IGet<>)
                   || type.GetGenericTypeDefinition() == typeof(IGetSet<>);
        }

        [CanBeNull]
        private static string LookupModelValue(NodeModel node, string fieldName)
        {
            node.Values.TryGetValue(fieldName, out var value);
            return value as string ?? fieldName;
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