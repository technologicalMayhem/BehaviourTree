using System.Reflection;
using System.Text.RegularExpressions;
using BehaviourTrees.Model;
using UnityEditor;
using UnityEngine.UIElements;

namespace BehaviourTrees.UnityEditor
{
    internal class EditorUtilities
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        internal static string LocateUiDefinitionFile(string className)
        {
            return $"Assets/Plugins/BehaviourTrees/Markup/{className}.uxml";
        }

        internal static StyleSheet GetStyleSheet()
        {
            return AssetDatabase.LoadAssetAtPath<StyleSheet>(LocateStyleSheet());
        }

        private static string LocateStyleSheet()
        {
            return "Assets/Plugins/BehaviourTrees/Markup/style.uss";
        }

        internal static string GetMemberName(MemberInfo nodeType)
        {
            var nameAttribute = nodeType.GetCustomAttribute<NodeNameAttribute>();
            if (nameAttribute != null)
            {
                return nameAttribute.Name;
            }
            
            var pascalRegex = new Regex(
                @"(?<=[A-Z])(?=[A-Z][a-z])|(?<=[^A-Z])(?=[A-Z])|(?<=[A-Za-z])(?=[^A-Za-z])"
            );

            return pascalRegex.Replace(nodeType.Name, " ");
        }
    }
}