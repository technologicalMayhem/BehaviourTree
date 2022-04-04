using BehaviourTrees.Model;
using UnityEditor;
using UnityEngine;

namespace BehaviourTrees.UnityEditor
{
    [CreateAssetMenu(fileName = "New Behaviour Tree", menuName = "Behaviour Tree", order = 250)]
    public class EditorTreeContainer : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField] [HideInInspector] private string SerializedTree;
        [SerializeField] [HideInInspector] private string SerializedExtensions;
        public EditorModelExtension ModelExtension = new EditorModelExtension();

        public BehaviourTreeModel TreeModel = new BehaviourTreeModel();

        public void OnBeforeSerialize()
        {
            SerializedTree = TreeModel.Serialize();
            SerializedExtensions = ModelExtension.Serialize();
        }

        public void OnAfterDeserialize()
        {
            TreeModel = ModelUtilities.Deserialize(SerializedTree);
            ModelExtension = EditorModelExtension.Deserialize(SerializedExtensions);
        }

        public void MarkDirty()
        {
            EditorUtility.SetDirty(this);
        }
    }
}