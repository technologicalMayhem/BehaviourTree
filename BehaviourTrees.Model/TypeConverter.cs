using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BehaviourTrees.Model
{
    internal class TypeConverter : JsonConverter<Type>
    {
        public override Type Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var s = reader.GetString();
            return s == null ? null : Type.GetType(s);
        }

        public override void Write(Utf8JsonWriter writer, Type value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.FullName);
        }
    }
}