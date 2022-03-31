using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Numerics;

namespace BehaviourTrees.Model
{
    public class NodeModel
    {
        public string Id;
        public Vector2 Position;
        public Type Type;
        public IReadOnlyDictionary<string, object> Values => new ReadOnlyDictionary<string, object>(_values);
        internal Dictionary<string, object> _values;
    }
}