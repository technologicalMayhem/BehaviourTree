using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace BehaviourTrees.UnityEditor.Converters
{
    /// <summary>
    ///     Converts a Vector3 object to and from JSON
    /// </summary>
    public class Vector3Converter : JsonConverter<Vector3>
    {
        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, Vector3 value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            new JProperty("x", value.x).WriteTo(writer);
            new JProperty("y", value.y).WriteTo(writer);
            new JProperty("z", value.z).WriteTo(writer);
            writer.WriteEndObject();
        }

        /// <inheritdoc />
        public override Vector3 ReadJson(JsonReader reader, Type objectType, Vector3 existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            var o = JObject.Load(reader);
            return new Vector3(o.Value<float>("x"), o.Value<float>("y"), o.Value<float>("z"));
        }
    }
}