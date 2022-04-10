using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace BehaviourTrees.UnityEditor.Converters
{
    /// <summary>
    ///     Converts a Vector4 object to and from JSON
    /// </summary>
    public class Vector4Converter : JsonConverter<Vector4>
    {
        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, Vector4 value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            new JProperty("x", value.x).WriteTo(writer);
            new JProperty("y", value.y).WriteTo(writer);
            new JProperty("z", value.z).WriteTo(writer);
            new JProperty("w", value.w).WriteTo(writer);
            writer.WriteEndObject();
        }

        /// <inheritdoc />
        public override Vector4 ReadJson(JsonReader reader, Type objectType, Vector4 existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            var o = JObject.Load(reader);
            return new Vector4(o.Value<float>("x"), o.Value<float>("y"), o.Value<float>("z"), o.Value<float>("w"));
        }
    }
}