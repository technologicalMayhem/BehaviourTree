using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BehaviourTrees.Core;
using Newtonsoft.Json;

namespace BehaviourTrees.Model
{
    public static class ModelUtilities
    {
        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            // Converters = { new TypeConverter() }
        };

        public static bool InheritsFrom<TBaseType>(this Type type)
        {
            return InheritsFrom(type, typeof(TBaseType));
        }

        public static bool InheritsFrom(this Type type, Type baseType)
        {
            if (type == null) return false;
            if (baseType.IsAssignableFrom(type))
                return true;
            if (type.IsInterface && !baseType.IsInterface)
                return false;
            if (baseType.IsInterface)
                return type.GetInterfaces().Contains(baseType);
            for (var type1 = type; type1 != null; type1 = type1.BaseType)
                if (type1 == baseType || baseType.IsGenericTypeDefinition && type1.IsGenericType &&
                    type1.GetGenericTypeDefinition() == baseType)
                    return true;
            return false;
        }

        public static IEnumerable<FieldInfo> GetFillableFields(
            this IBehaviourTreeNode node)
        {
            return node.GetType().GetFields();
        }

        public static string Serialize(this BehaviourTreeModel treeModel)
        {
            return JsonConvert.SerializeObject(treeModel, Settings);
        }

        public static BehaviourTreeModel Deserialize(string serializedModel)
        {
            var conceptualBehaviourTree = JsonConvert.DeserializeObject<BehaviourTreeModel>(serializedModel, Settings);
            return conceptualBehaviourTree;
        }
    }
}