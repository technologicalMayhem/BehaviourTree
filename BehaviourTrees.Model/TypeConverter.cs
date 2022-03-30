using System;
using Newtonsoft.Json;

namespace BehaviourTrees.Model
{
    // internal class TypeConverter : JsonConverter<Type>
    // {
    //     public override Type ReadJson(JsonReader reader, Type objectType, Type existingValue, bool hasExistingValue,
    //         JsonSerializer serializer)        {
    //         var s = reader.ReadAsString();
    //         return s == null ? null : Type.GetType(s);
    //     }
    //
    //     public override void WriteJson(JsonWriter writer, Type value, JsonSerializer serializer)
    //     {
    //         writer.WriteValue(value.FullName);
    //     }
    // }
}