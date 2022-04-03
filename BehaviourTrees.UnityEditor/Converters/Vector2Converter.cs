using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace BehaviourTrees.UnityEditor.Converters
{
    
    /// <summary>
    /// Converts a Vector2 object to and from JSON
    /// </summary>
    public class Vector2Converter : JsonConverter<Vector2>
    {
        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, Vector2 value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            new JProperty("x", value.x).WriteTo(writer);
            new JProperty("y", value.y).WriteTo(writer);
            writer.WriteEndObject();
        }

        /// <inheritdoc />
        public override Vector2 ReadJson(JsonReader reader, Type objectType, Vector2 existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var o = JObject.Load(reader);
            return new Vector2(o.Value<float>("x"), o.Value<float>("y"));
        }
    }
}