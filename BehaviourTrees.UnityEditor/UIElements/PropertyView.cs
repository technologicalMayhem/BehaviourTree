using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using UnityObject = UnityEngine.Object;

namespace BehaviourTrees.UnityEditor.UIElements
{
    public class PropertyView : VisualElement
    {
        private readonly VisualElement _editorElement;

        private readonly Label _name;
        private Type _type;
        public EditorTreeContainer Tree;

        public PropertyView()
        {
            var visualTree = TreeEditorUtility.GetVisualTree(nameof(PropertyView));
            visualTree.CloneTree(this);

            styleSheets.Add(TreeEditorUtility.GetStyleSheet());

            _name = this.Q<Label>("property-name");
            _editorElement = this.Q<VisualElement>("property-value");
        }

        public static PropertyView CreateEditor(string name, Type type, object value, Action<object> callback)
        {
            var propertyView = new PropertyView();

            propertyView._name.text = name;
            propertyView._editorElement.Add(propertyView.GetEditorElement(type, value, callback));

            return propertyView;
        }


        public static PropertyView CreateBlackboardDropdown(EditorTreeContainer tree, string name,
            Type blackboardType, string key, Action<object> callback, BlackboardView blackboardView)
        {
            var propertyView = new PropertyView();

            propertyView._name.text = name;
            propertyView._editorElement.Add(new BlackboardDropdown(tree, blackboardType, key, callback));

            return propertyView;
        }

        private VisualElement GetEditorElement(Type type, object value, Action<object> callback)
        {
            if (type == typeof(string))
                return CreateElementWithCallback<TextField, string>(value, callback);

            if (type == typeof(int))
                return CreateElementWithCallback<IntegerField, int>(value, callback);

            if (type == typeof(long))
                return CreateElementWithCallback<LongField, long>(value, callback);

            if (type == typeof(float))
                return CreateElementWithCallback<FloatField, float>(value, callback);

            if (type == typeof(double))
                return CreateElementWithCallback<DoubleField, double>(value, callback);

            if (type == typeof(Vector2))
                return CreateElementWithCallback<Vector2Field, Vector2>(value, callback);

            if (type == typeof(Vector3))
                return CreateElementWithCallback<Vector3Field, Vector3>(value, callback);

            if (type == typeof(Vector4))
                return CreateElementWithCallback<Vector4Field, Vector4>(value, callback);

            if (type == typeof(Rect))
                return CreateElementWithCallback<RectField, Rect>(value, callback);

            if (TypeCache.GetTypesDerivedFrom<GameObject>().Contains(type) ||
                TypeCache.GetTypesDerivedFrom<ScriptableObject>().Contains(type))
                return CreateElementWithCallback<ObjectField, UnityObject>(value, callback);

            var label = new Label($"No editor for {type} found.");
            label.style.color = new StyleColor(Color.red);
            return label;
        }

        private VisualElement CreateElementWithCallback<TField, TValue>(object value, Action<object> callback)
            where TField : BaseField<TValue>, new()
        {
            var editorElement = new TField
            {
                //Due to boxing, a direct cast might not work, so this prevents an exception being thrown.
                value = (TValue)Convert.ChangeType(value, typeof(TValue))
            };
            editorElement.RegisterValueChangedCallback(evt => callback(evt.newValue));
            return editorElement;
        }

        public new class UxmlFactory : UxmlFactory<PropertyView, UxmlTraits> { }
    }
}