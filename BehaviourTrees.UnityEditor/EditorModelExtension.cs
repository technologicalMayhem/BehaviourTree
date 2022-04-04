using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace BehaviourTrees.UnityEditor
{
    public class EditorModelExtension
    {
        public Dictionary<string, Type> BlackboardKeys = new Dictionary<string, Type>();
        public Dictionary<string, Vector2> NodePositions = new Dictionary<string, Vector2>();

        public void InvokeBlackboardKeysChanged(object sender)
        {
            BlackboardKeysChanged?.Invoke(sender, EventArgs.Empty);
        }

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this, TreeEditorUtility.SerializerSettings);
        }

        public static EditorModelExtension Deserialize(string data)
        {
            return JsonConvert.DeserializeObject<EditorModelExtension>(data, TreeEditorUtility.SerializerSettings);
        }

        public event EventHandler BlackboardKeysChanged;
    }
}