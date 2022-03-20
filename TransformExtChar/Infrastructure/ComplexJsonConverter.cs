using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace TransformExtChar.Infrastructure
{
    public class ComplexJsonConverter : JsonConverter
    {
        // By using a surrogate type, we respect the naming conventions of the serializer's contract resolver.
        class ComplexSurrogate 
        {
            [JsonProperty(Required = Required.Always)]
            public double Real { get; set; }

            [JsonProperty(Required = Required.Always)]
            public double Imaginary { get; set; }

            public static implicit operator Complex(ComplexSurrogate surrogate)
            {
                if (surrogate == null)
                    return default(Complex);
                return new Complex(surrogate.Real, surrogate.Imaginary);
            }

            public static implicit operator ComplexSurrogate(Complex complex)
            {
                return new ComplexSurrogate { Real = complex.Real, Imaginary = complex.Imaginary };
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Complex) || objectType == typeof(Complex?);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;
            return (Complex)serializer.Deserialize<ComplexSurrogate>(reader);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, (ComplexSurrogate)(Complex)value);
        }
    }
}
