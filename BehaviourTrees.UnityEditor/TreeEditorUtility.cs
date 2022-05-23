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
    /// <summary>
    ///     Provides some common utility methods for the editor as well looks ups resources.
    /// </summary>
    internal static class TreeEditorUtility
    {
        /// <summary>
        ///     The base path of all editor resources.
        /// </summary>
        private const string BasePath =
            "Packages/" + PackageName + "/Runtime/BehaviourTrees.UnityEditor";

        private const string PackageName =
            "com.github.technologicalmayhem.behaviourtree";

        /// <summary>
        ///     The location of the style sheet.
        /// </summary>
        private static string StyleSheetLocation => $"{BasePath}/Markup/style.uss";

        /// <summary>
        ///     Formats a type name in a user friendly way.
        /// </summary>
        /// <param name="type">The type to convert.</param>
        /// <returns>The friendly name of the type.</returns>
        public static string GetTypeName(Type type)
        {
            if (!type.IsConstructedGenericType) return type.Name;
            if (!type.IsGenericType) return type.Name.Split('`').First();

            var baseName = type.Name.Split('`').First();
            var parameters = string.Join(", ", type.GetGenericArguments().Select(t => t.Name));

            return $"{baseName}<{parameters}>";
        }

        /// <summary>
        ///     Checks if a field in a type is used to get a key from the blackboard.
        /// </summary>
        /// <param name="type">The type too do the check for.</param>
        /// <param name="fieldName">The name of the field to check.</param>
        /// <param name="blackboardType">The type expected to be returned for the blackboard key.</param>
        /// <returns>True if the field is used as a key for the blackboard.</returns>
        public static bool IsBlackboardField(Type type, string fieldName, out Type blackboardType)
        {
            var fieldInfo = type.GetField(fieldName);
            if (fieldInfo == null)
            {
                blackboardType = null;
                return false;
            }

            var attribute = fieldInfo.GetCustomAttribute<BlackboardDropdownAttribute>();
            blackboardType = attribute?.Type;
            return blackboardType != null;
        }

        /// <summary>
        ///     Gets the visual tree for the given class name.
        /// </summary>
        /// <param name="className">The name of the class.</param>
        /// <returns>The visual tree for the class.</returns>
        internal static VisualTreeAsset GetVisualTree(string className)
        {
            return AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(LocateUiDefinitionFile(className));
        }

        /// <summary>
        ///     Returns the uxml file path for the class.
        /// </summary>
        /// <param name="className">The name of the class.</param>
        /// <returns>The path to the uxml file.</returns>
        internal static string LocateUiDefinitionFile(string className)
        {
            return $"{BasePath}/Markup/{className}.uxml";
        }

        /// <summary>
        ///     Gets the style sheet.
        /// </summary>
        /// <returns>The style sheet.</returns>
        internal static StyleSheet GetStyleSheet()
        {
            return AssetDatabase.LoadAssetAtPath<StyleSheet>(StyleSheetLocation);
        }

        /// <summary>
        ///     Gets the icon for the editor.
        /// </summary>
        /// <returns>The texture for the editor icon.</returns>
        internal static Texture GetEditorIcon()
        {
            return AssetDatabase.LoadAssetAtPath<Texture>($"{BasePath}/EditorIcon.png");
        }

        /// <summary>
        ///     Gets the name to be shown in the editor for a behaviour tree node.
        /// </summary>
        /// <param name="nodeType">The behaviour tree node type to get the name for.</param>
        /// <returns>A friendly name for the node type.</returns>
        internal static string GetNodeName(Type nodeType)
        {
            var nameAttribute = nodeType.GetCustomAttribute<NodeNameAttribute>();
            return nameAttribute != null ? nameAttribute.Name : SplitPascalCase(nodeType.Name);
        }

        /// <summary>
        ///     Splits a string using pascal case into individual words.
        /// </summary>
        /// <param name="pascalCaseString">The string to split.</param>
        /// <returns>The string that has been split up.</returns>
        internal static string SplitPascalCase(string pascalCaseString)
        {
            var pascalRegex = new Regex(
                @"(?<=[A-Z])(?=[A-Z][a-z])|(?<=[^A-Z])(?=[A-Z])|(?<=[A-Za-z])(?=[^A-Za-z])"
            );

            return pascalRegex.Replace(pascalCaseString, " ");
        }

        /// <summary>
        ///     Settings used for serialization throughout the entire project.
        /// </summary>
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            Converters = new JsonConverter[]
            {
                new RectConverter(),
                new Vector2Converter(),
                new Vector3Converter(),
                new Vector4Converter()
            }
        };
    }
}