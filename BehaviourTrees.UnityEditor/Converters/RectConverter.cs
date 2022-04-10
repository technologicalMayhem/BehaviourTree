using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace BehaviourTrees.UnityEditor.Converters
{
    /// <summary>
    ///     Converts a Rect object to and from JSON
    /// </summary>
    public class RectConverter : JsonConverter<Rect>
    {
        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, Rect value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            new JProperty("x", value.x).WriteTo(writer);
            new JProperty("y", value.y).WriteTo(writer);
            new JProperty("height", value.height).WriteTo(writer);
            new JProperty("width", value.width).WriteTo(writer);
            writer.WriteEndObject();
        }

        /// <inheritdoc />
        public override Rect ReadJson(JsonReader reader, Type objectType, Rect existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            var o = JObject.Load(reader);
            return new Rect(o.Value<float>("x"), o.Value<float>("y"), o.Value<float>("height"),
                o.Value<float>("width"));
        }
    }
}