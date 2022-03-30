using BehaviourTrees.Model;
using UnityEngine;

namespace BehaviourTrees.UnityEditor
{
    [CreateAssetMenu(fileName = "New Behaviour Tree", menuName = "Behaviour Tree", order = 250)]
    public class EditorTreeContainer : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField] [HideInInspector] private string SerializedTree;

        public ConceptualBehaviourTree TreeModel;

        public void OnBeforeSerialize()
        {
            SerializedTree = TreeModel.Serialize();
        }

        public void OnAfterDeserialize()
        {
            TreeModel = ModelUtilities.Deserialize(SerializedTree);
        }
    }
}