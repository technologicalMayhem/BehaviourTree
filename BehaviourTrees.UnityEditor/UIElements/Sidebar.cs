using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using BehaviourTrees.Model;
using UnityEngine.UIElements;

namespace BehaviourTrees.UnityEditor.UIElements
{
    /// <summary>
    ///     Displays <see cref="SidebarElement" />s. Individual elements can be collapsed.
    /// </summary>
    public class Sidebar : VisualElement
    {
        /// <summary>
        ///     The <see cref="ScrollView" /> containing the <see cref="SidebarElement" />s shown in the sidebar.
        /// </summary>
        private readonly ScrollView _scrollView;

        /// <summary>
        ///     A collection of the loaded <see cref="SidebarElement" />s.
        /// </summary>
        private readonly IReadOnlyCollection<SidebarElement> _sidebarElements;

        /// <summary>
        ///     Creates a new instance of the Sidebar element.
        /// </summary>
        public Sidebar()
        {
            var visualTree = TreeEditorUtility.GetVisualTree(nameof(Sidebar));
            visualTree.CloneTree(this);

            _scrollView = this.Q<ScrollView>("sidebar-elements");
            _sidebarElements = CreateSidebarElements();
            CreateSidebarElementContainers();
        }

        /// <summary>
        ///     Retrieves a element from the sidebar.
        /// </summary>
        /// <typeparam name="T">The type of the element.</typeparam>
        /// <returns>The requested element.</returns>
        public T GetElement<T>() where T : SidebarElement
        {
            return _sidebarElements.First(element => element is T) as T;
        }

        /// <summary>
        ///     Finds all types implementing <see cref="SidebarElement" /> and creates a instance of them.
        /// </summary>
        /// <returns>The created instances of <see cref="SidebarElement" />s.</returns>
        private static ReadOnlyCollection<SidebarElement> CreateSidebarElements()
        {
            var sidebarElements = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => !type.IsAbstract && type.InheritsFrom<SidebarElement>());

            return sidebarElements
                .Select(type => (SidebarElement)Activator.CreateInstance(type))
                .OrderByDescending(element => element.DefaultPosition)
                .ToList()
                .AsReadOnly();
        }

        /// <summary>
        ///     Creates a <see cref="SidebarElementContainer" /> for each element in <see cref="_sidebarElements" /> and adds
        ///     it to <see cref="_scrollView" />.
        /// </summary>
        private void CreateSidebarElementContainers()
        {
            foreach (var sidebarElement in _sidebarElements)
                _scrollView.Add(new SidebarElementContainer(sidebarElement));
        }

        /// <summary>
        ///     Instantiates a <see cref="Sidebar" /> using the data read from a UXML file
        /// </summary>
        public new class UxmlFactory : UxmlFactory<Sidebar, UxmlTraits> { }
    }
}