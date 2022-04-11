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
        private const string BasePath =
            "Packages/com.github.technologicalmayhem.behaviourtree/Editor";
        
        internal static JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            Converters =
            {
                new Vector2Converter()
            }
        };

        public static string GetTypeName(Type type)
        {
            if (!type.IsConstructedGenericType) return type.Name;
            if (!type.IsGenericType) return type.Name.Split('`').First();

            var baseName = type.Name.Split('`').First();
            var parameters = string.Join(", ", type.GetGenericArguments().Select(t => t.Name));

            return $"{baseName}<{parameters}>";
        }

        public static bool IsBlackboardField(Type type, string fieldName, out Type blackboardType)
        {
            var attribute = type.GetField(fieldName).GetCustomAttribute<BlackboardDropdownAttribute>();
            blackboardType = attribute?.Type;
            return blackboardType != null;
        }

        internal static VisualTreeAsset LoadPartialUi(string className, string partialName)
        {
            return AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                LocatePartialUiDefinitionFile(className, partialName));
        }

        internal static string LocateUiDefinitionFile(string className)
        {
            return $"{BasePath}/Markup/{className}.uxml";
        }

        internal static StyleSheet GetStyleSheet()
        {
            return AssetDatabase.LoadAssetAtPath<StyleSheet>(LocateStyleSheet());
        }

        internal static Texture GetEditorIcon()
        {
            return AssetDatabase.LoadAssetAtPath<Texture>($"{BasePath}/EditorIcon.png");
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

        private static string LocatePartialUiDefinitionFile(string className, string partialName)
        {
            return $"{BasePath}/Markup/{className}/{partialName}.uxml";
        }

        private static string LocateStyleSheet()
        {
            return $"{BasePath}/Markup/style.uss";
        }
    }
}