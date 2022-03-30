using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.Serialization;
using System.Text;
using BehaviourTrees.Core;

namespace BehaviourTrees.Model
{
    public sealed class ConceptualBehaviourTree
    {
        private readonly Random _random = new Random();
        public readonly Dictionary<string, IList<string>> Connections;
        public readonly List<NodeModel> Nodes;

        public ConceptualBehaviourTree()
        {
            Nodes = new List<NodeModel>();
            Connections = new Dictionary<string, IList<string>>();
        }

        [OnDeserialized]
        public void OnDeserialized(StreamingContext context)
        {
            foreach (var nodeModel in Nodes) EnsureDictionaryIsCorrect(nodeModel);
        }

        public NodeModel CreateNode<T>(float posX = 0, float posY = 0)
        {
            return CreateNode(typeof(T), posX, posY);
        }

        public NodeModel CreateNode(Type type, float posX = 0, float posY = 0)
        {
            if (!type.InheritsFrom(typeof(BehaviourTreeNode)))
                throw new ArgumentException(
                    $"Type must be inheriting from {typeof(BehaviourTreeNode)}, but {type} doesn't.");
            var model = new NodeModel
            {
                Type = type,
                Position = new Vector2(posX, posY),
                Id = NewUid()
            };
            EnsureDictionaryIsCorrect(model);
            AddNodeToTree(model);

            return model;
        }

        public void AddChild(NodeModel parentNode, NodeModel childNode, int parentPortIndex = -1)
        {
            if (childNode.Type == typeof(RootNode))
                throw new InvalidOperationException("Root node can't be a child to another node");

            if (parentNode.Type.InheritsFrom(typeof(LeafNode<>)))
                throw new InvalidOperationException("Leaf nodes cannot have children.");

            if (Connections.TryGetValue(parentNode.Id, out var list))
            {
                //Make sure the desired index is within range, otherwise add it to the end
                if (parentPortIndex >= 0 && parentPortIndex < list.Count)
                    list.Insert(parentPortIndex, childNode.Id);
                else
                    list.Add(childNode.Id);
            }
            else
            {
                Connections[parentNode.Id] = new List<string> { childNode.Id };
            }

            FireTreeChangedEvent();
        }

        public void RemoveChild(NodeModel parentNode, NodeModel childNode)
        {
            if (childNode.Type == typeof(RootNode))
                throw new InvalidOperationException("Root node can't be a child to another node");

            if (parentNode.Type.InheritsFrom(typeof(LeafNode<>)))
                throw new InvalidOperationException("Leaf nodes cannot have children.");

            Connections[parentNode.Id].Remove(childNode.Id);
            if (!Connections[parentNode.Id].Any()) Connections.Remove(parentNode.Id);

            FireTreeChangedEvent();
        }

        public IEnumerable<NodeModel> GetChildren(NodeModel parentNode)
        {
            if (Connections.ContainsKey(parentNode.Id))
                return Connections[parentNode.Id]
                    .Select(s => Nodes.First(model => model.Id == s))
                    .ToList();

            return new List<NodeModel>();
        }

        public NodeModel GetById(string id)
        {
            return Nodes.First(model => model.Id == id);
        }

        public void Delete(NodeModel node)
        {
            Nodes.Remove(node);
            FireTreeChangedEvent();
        }

        public void MoveNode(NodeModel node, float x, float y)
        {
            node.Position = new Vector2(x, y);
            FireTreeChangedEvent();
        }

        private static void EnsureDictionaryIsCorrect(NodeModel model)
        {
            model.Values ??= new Dictionary<string, object>();

            foreach (var field in model.Type.GetFields().Where(field => field.IsPublic))
            {
                if (field.FieldType.IsInterface) continue;
                if (field.GetCustomAttributes(true).Any(o => o is ExcludeFromEditorAttribute)) continue;
                if (!model.Values.ContainsKey(field.Name))
                {
                    var value =
                        field.FieldType == typeof(string) ? string.Empty : Activator.CreateInstance(field.FieldType);
                    model.Values.Add(field.Name, value);
                }
            }
        }

        private string NewUid()
        {
            while (true)
            {
                var chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz".ToCharArray();
                var sb = new StringBuilder(6);

                for (var i = 0; i < 6; i++) sb.Append(chars[_random.Next(62)]);

                var newUid = sb.ToString();

                //Ensure it is actually unique
                if (Nodes.Any(model => model.Id == newUid)) continue;
                return newUid;
            }
        }

        private void AddNodeToTree(NodeModel model)
        {
            Nodes.Add(model);
            FireTreeChangedEvent();
        }

        private void FireTreeChangedEvent()
        {
            TreeHasChanged?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler TreeHasChanged;
    }
}