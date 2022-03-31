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
        private readonly Label _name;
        private readonly VisualElement _value;
        private Action<object> _callback;
        private EditorTreeContainer _container;
        private Action<object> _setValue;

        public PropertyView()
        {
            var visualTree =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    EditorUtilities.LocateUiDefinitionFile(nameof(PropertyView)));
            visualTree.CloneTree(this);
            
            styleSheets.Add(EditorUtilities.GetStyleSheet());

            _name = this.Q<Label>("property-name");
            _value = this.Q<VisualElement>("property-value");
        }

        public void CreateEditor(EditorTreeContainer containerReference, string propertyName, Type type,
            Action<object> callback)
        {
            _container = containerReference;
            _name.text = EditorUtilities.SplitPascalCase(propertyName);
            _callback = callback;
            _value.Add(GetEditorElement(type));
        }

        public void SetValue(object value)
        {
            Undo.RecordObject(_container, "Change Value on Node");
            _setValue.Invoke(value);
        }

        private VisualElement GetEditorElement(Type type)
        {
            if (type == typeof(string))
                return CreateElementWithCallback<TextField, string>();

            if (type == typeof(int))
                return CreateElementWithCallback<IntegerField, int>();

            if (type == typeof(long))
                return CreateElementWithCallback<LongField, long>();

            if (type == typeof(float))
                return CreateElementWithCallback<FloatField, float>();

            if (type == typeof(double))
                return CreateElementWithCallback<DoubleField, double>();

            if (type == typeof(Vector2))
                return CreateElementWithCallback<Vector2Field, Vector2>();

            if (type == typeof(Vector3))
                return CreateElementWithCallback<Vector3Field, Vector3>();

            if (type == typeof(Vector4))
                return CreateElementWithCallback<Vector4Field, Vector4>();

            if (type == typeof(Rect))
                return CreateElementWithCallback<RectField, Rect>();

            if (TypeCache.GetTypesDerivedFrom<GameObject>().Contains(type) ||
                TypeCache.GetTypesDerivedFrom<ScriptableObject>().Contains(type))
                return CreateElementWithCallback<ObjectField, UnityObject>();

            var label = new Label($"No editor for {type} found.");
            label.style.color = new StyleColor(Color.red);
            return label;
        }

        private VisualElement CreateElementWithCallback<TField, TValue>() where TField : BaseField<TValue>, new()
        {
            var editorElement = new TField();
            editorElement.RegisterValueChangedCallback(evt => _callback.Invoke(evt.newValue));
            _setValue = value => editorElement.value = (TValue)value;
            return editorElement;
        }

        public new class UxmlFactory : UxmlFactory<PropertyView, UxmlTraits> { }
    }
}