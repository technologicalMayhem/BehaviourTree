using BehaviourTrees.Model;
using BehaviourTrees.UnityEditor.Converters;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace BehaviourTrees.UnityEditor
{
    [CreateAssetMenu(fileName = "New Behaviour Tree", menuName = "Behaviour Tree", order = 250)]
    public class EditorTreeContainer : ScriptableObject, ISerializationCallbackReceiver
    {
        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            Converters = new JsonConverter[]
            {
                new Vector2Converter()
            }
        };

        [SerializeField] [HideInInspector] private string SerializedTree;
        [SerializeField] [HideInInspector] private string SerializedExtensions;

        public EditorModelExtension ModelExtension = new EditorModelExtension();
        public BehaviourTreeModel TreeModel = new BehaviourTreeModel();

        public void OnBeforeSerialize()
        {
            string tree;
            string extension;

            try
            {
                tree = JsonConvert.SerializeObject(TreeModel, Settings);
                extension = JsonConvert.SerializeObject(ModelExtension, Settings);
            }
            catch (JsonSerializationException e)
            {
                Debug.LogError($"An error occured during serialization of the tree container. {e.Message}", this);
                return;
            }

            SerializedTree = tree;
            SerializedExtensions = extension;
        }

        public void OnAfterDeserialize()
        {
            BehaviourTreeModel tree;
            EditorModelExtension extension;

            try
            {
                tree = JsonConvert.DeserializeObject<BehaviourTreeModel>(SerializedTree, Settings);
                extension = JsonConvert.DeserializeObject<EditorModelExtension>(SerializedExtensions, Settings);
            }
            catch (JsonSerializationException e)
            {
                Debug.LogError($"An error occured during deserialization of the tree container. {e.Message}", this);
                return;
            }

            TreeModel = tree;
            ModelExtension = extension;
        }

        public void MarkDirty()
        {
            EditorUtility.SetDirty(this);
        }
    }
}