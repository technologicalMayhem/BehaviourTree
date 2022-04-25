using BehaviourTrees.Model;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace BehaviourTrees.UnityEditor.Data
{
    /// <summary>
    ///     Contains the data that the <see cref="BehaviourTreeEditor" /> used to create and edit behaviour trees, as well
    ///     as managing the serialization of the data within Unity.
    /// </summary>
    [CreateAssetMenu(fileName = "New Behaviour Tree", menuName = "Behaviour Tree", order = 250)]
    public class EditorTreeContainer : ScriptableObject, ISerializationCallbackReceiver
    {
        /// <summary>
        ///     Represents the <see cref="BehaviourTreeModel" /> in serialized form.
        /// </summary>
        [SerializeField] [HideInInspector] private string SerializedTree;

        /// <summary>
        ///     Represents the <see cref="ModelExtension" /> in serialized form.
        /// </summary>
        [SerializeField] [HideInInspector] private string SerializedExtensions;

        /// <inheritdoc cref="EditorModelExtension" />
        public EditorModelExtension ModelExtension = new EditorModelExtension();

        /// <inheritdoc cref="BehaviourTreeModel" />
        public BehaviourTreeModel TreeModel = new BehaviourTreeModel();

        /// <inheritdoc />
        public void OnBeforeSerialize()
        {
            string tree;
            string extension;

            try
            {
                tree = JsonConvert.SerializeObject(TreeModel, TreeEditorUtility.Settings);
                extension = JsonConvert.SerializeObject(ModelExtension, TreeEditorUtility.Settings);
            }
            catch (JsonSerializationException e)
            {
                Debug.LogError($"An error occured during serialization of the tree container. {e.Message}", this);
                return;
            }

            SerializedTree = tree;
            SerializedExtensions = extension;
        }

        /// <inheritdoc />
        public void OnAfterDeserialize()
        {
            BehaviourTreeModel tree;
            EditorModelExtension extension;

            try
            {
                tree = JsonConvert.DeserializeObject<BehaviourTreeModel>(SerializedTree, TreeEditorUtility.Settings);
                extension = JsonConvert.DeserializeObject<EditorModelExtension>(SerializedExtensions,
                    TreeEditorUtility.Settings);
            }
            catch (JsonSerializationException e)
            {
                Debug.LogError($"An error occured during deserialization of the tree container. {e.Message}", this);
                return;
            }

            TreeModel = tree;
            ModelExtension = extension;
        }

        /// <summary>
        ///     Marks that edits have been made to the container that need to be saved.
        /// </summary>
        public void MarkDirty()
        {
            EditorUtility.SetDirty(this);
        }
    }
}