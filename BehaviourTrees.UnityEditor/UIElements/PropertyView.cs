using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using UnityObject = UnityEngine.Object;

namespace BehaviourTrees.UnityEditor.UIElements
{
    /// <summary>
    ///     Represents a property to be edited.
    /// </summary>
    public class PropertyView : VisualElement
    {
        /// <summary>
        ///     The visual element of the editor.
        /// </summary>
        private readonly VisualElement _editorElement;

        /// <summary>
        ///     The label with the property name.
        /// </summary>
        private readonly Label _name;

        /// <summary>
        ///     Creates a new instance of the PropertyView element.
        /// </summary>
        public PropertyView()
        {
            var visualTree = TreeEditorUtility.GetVisualTree(nameof(PropertyView));
            visualTree.CloneTree(this);

            styleSheets.Add(TreeEditorUtility.GetStyleSheet());

            _name = this.Q<Label>("property-name");
            _editorElement = this.Q<VisualElement>("property-value");
        }

        /// <summary>
        ///     Creates a new instance of <see cref="PropertyView" /> to edit a property.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="type">The type of the property.</param>
        /// <param name="value">The value of the property.</param>
        /// <param name="callback">The callback to invoke when the value has changed.</param>
        /// <returns>A instance of <see cref="PropertyView" /> for the property.</returns>
        public static PropertyView CreateEditor(string name, Type type, object value, Action<object> callback)
        {
            var propertyView = new PropertyView();

            propertyView._name.text = name;
            propertyView._editorElement.Add(propertyView.GetEditorElement(type, value, callback));

            return propertyView;
        }

        /// <summary>
        ///     Creates a new instance of <see cref="PropertyView" /> to edit a property for a blackboard key.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="blackboardType">The type of the blackboard key.</param>
        /// <param name="key">The current key.</param>
        /// <param name="callback">The callback to invoke when the value has changed.</param>
        /// <returns>A instance of <see cref="PropertyView" /> for the property.</returns>
        public static PropertyView CreateBlackboardDropdown(string name,
            Type blackboardType, string key, Action<object> callback)
        {
            var propertyView = new PropertyView();

            propertyView._name.text = name;
            propertyView._editorElement.Add(new BlackboardDropdown(blackboardType, key, callback));

            return propertyView;
        }

        /// <summary>
        ///     Creates a editor of the type given to this <see cref="PropertyView" />.
        /// </summary>
        /// <param name="type">The type to create an editor for.</param>
        /// <param name="value">The initial value.</param>
        /// <param name="callback">A callback that is fired if the value changed.</param>
        /// <returns>A editor for the value.</returns>
        private VisualElement GetEditorElement(Type type, object value, Action<object> callback)
        {
            if (type == typeof(string))
                return CreateEditorField<TextField, string>(value, callback);

            if (type == typeof(int))
                return CreateEditorField<IntegerField, int>(value, callback);

            if (type == typeof(long))
                return CreateEditorField<LongField, long>(value, callback);

            if (type == typeof(float))
                return CreateEditorField<FloatField, float>(value, callback);

            if (type == typeof(double))
                return CreateEditorField<DoubleField, double>(value, callback);

            if (type == typeof(Vector2))
                return CreateEditorField<Vector2Field, Vector2>(value, callback);

            if (type == typeof(Vector3))
                return CreateEditorField<Vector3Field, Vector3>(value, callback);

            if (type == typeof(Vector4))
                return CreateEditorField<Vector4Field, Vector4>(value, callback);

            if (type == typeof(Rect))
                return CreateEditorField<RectField, Rect>(value, callback);

            if (TypeCache.GetTypesDerivedFrom<GameObject>().Contains(type) ||
                TypeCache.GetTypesDerivedFrom<ScriptableObject>().Contains(type))
                return CreateEditorField<ObjectField, UnityObject>(value, callback);

            var label = new Label($"No editor for {type} found.");
            label.style.color = new StyleColor(Color.red);
            return label;
        }

        /// <summary>
        ///     Creates a new <see cref="BaseField{TValueType}" /> of the given type, initializes it with a value and
        ///     registers a callback for when a value change occurs.
        /// </summary>
        /// <param name="value">The initial value to set in the <see cref="BaseField{TValueType}" /></param>
        /// <param name="callback">The call to be invoked when the value changed.</param>
        /// <typeparam name="TField">The type of the <see cref="BaseField{TValueType}" /> to create.</typeparam>
        /// <typeparam name="TValue">The type of the value the field is for.</typeparam>
        /// <returns>A instance of <see cref="BaseField{TValueType}" /> with a initial value set and callback registered.</returns>
        private static VisualElement CreateEditorField<TField, TValue>(object value, Action<object> callback)
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

        /// <summary>
        ///     Instantiates a <see cref="PropertyView" /> using the data read from a UXML file
        /// </summary>
        public new class UxmlFactory : UxmlFactory<PropertyView, UxmlTraits> { }
    }
}