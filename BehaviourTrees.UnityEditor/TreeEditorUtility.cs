using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using BehaviourTrees.Model;
using BehaviourTrees.UnityEditor.Converters;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviourTrees.UnityEditor
{
    internal static class TreeEditorUtility
    {
        internal static JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            Converters =
            {
                new Vector2Converter()
            }
        };
        internal static VisualTreeAsset LoadPartialUi(string className, string partialName)
        {
            return AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(LocatePartialUiDefinitionFile(className, partialName));
        }
        internal static string LocateUiDefinitionFile(string className)
        {
            return $"Assets/Plugins/BehaviourTrees/Markup/{className}.uxml";
        }

        private static string LocatePartialUiDefinitionFile(string className, string partialName)
        {
            return $"Assets/Plugins/BehaviourTrees/Markup/{className}/{partialName}.uxml";
        }

        internal static StyleSheet GetStyleSheet()
        {
            return AssetDatabase.LoadAssetAtPath<StyleSheet>(LocateStyleSheet());
        }

        private static string LocateStyleSheet()
        {
            return "Assets/Plugins/BehaviourTrees/Markup/style.uss";
        }

        internal static Texture GetEditorIcon()
        {
            return AssetDatabase.LoadAssetAtPath<Texture>("Assets/Plugins/BehaviourTrees/EditorIcon.png");
        }

        internal static string GetMemberName(MemberInfo nodeType)
        {
            var nameAttribute = nodeType.GetCustomAttribute<NodeNameAttribute>();
            return nameAttribute != null ? nameAttribute.Name : SplitPascalCase(nodeType.Name);
        }

        internal static string SplitPascalCase(string pascalCaseString)
        {
            var pascalRegex = new Regex(
                @"(?<=[A-Z])(?=[A-Z][a-z])|(?<=[^A-Z])(?=[A-Z])|(?<=[A-Za-z])(?=[^A-Za-z])"
            );

            return pascalRegex.Replace(pascalCaseString, " ");
        }

        public static string GetTypeName(Type type)
        {
            if (!type.IsConstructedGenericType) return type.Name;
            if (!type.IsGenericType) return type.Name.Split('`').First();
            
            var baseName = type.Name.Split('`').First();
            var parameters = string.Join(", ", type.GetGenericArguments().Select(t => t.Name));
            
            return $"{baseName}<{parameters}>";
        }
    }
}