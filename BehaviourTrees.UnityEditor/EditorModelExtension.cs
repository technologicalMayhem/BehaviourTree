using System;
using System.Collections.Generic;
using BehaviourTrees.Model;
using Newtonsoft.Json;
using UnityEngine;

namespace BehaviourTrees.UnityEditor
{
    public class EditorModelExtension
    {
        public Dictionary<string, Vector2> NodePositions = new Dictionary<string, Vector2>();
        public Dictionary<string, Type> BlackboardKeys = new Dictionary<string, Type>();

        public event EventHandler BlackboardKeysChanged;

        public void InvokeBlackboardKeysChanged(object sender)
        {
            BlackboardKeysChanged?.Invoke(sender, EventArgs.Empty);
        }
        
        public string Serialize() => JsonConvert.SerializeObject(this, TreeEditorUtility.SerializerSettings);
        public static EditorModelExtension Deserialize(string data) 
            => JsonConvert.DeserializeObject<EditorModelExtension>(data, TreeEditorUtility.SerializerSettings);
    }
}