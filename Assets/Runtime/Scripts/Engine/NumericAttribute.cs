using System;
using System.Collections;
using System.Collections.Generic;
using BreakInfinity;
using Newtonsoft.Json;
using UnityEngine;
namespace io.github.thisisnozaku.idle.framework.Engine
{
    /*
     * This is a convenience class for a common scenario in many idle games: a numeric property which has a base value,
     * a level which is multiplied by some per-level delta and added on top of the base value; this value is then finally
     * multiplied by some value.
     */
    [JsonConverter(typeof(NumericAttributeConverter))]
    public class NumericAttribute
    {
        [JsonProperty]
        public BigDouble BaseValue;
        [JsonProperty]
        public BigDouble Multiplier;
        [JsonProperty]
        public BigDouble ChangePerLevel;
        [JsonProperty]
        public BigDouble Level;

        [JsonConstructor]
        public NumericAttribute(BigDouble perLevelValue, BigDouble? multiplier = null, BigDouble? baseValue = null, BigDouble? level = null)
        {
            this.ChangePerLevel = perLevelValue;
            this.BaseValue = baseValue.HasValue ? baseValue.Value : this.BaseValue;
            this.Multiplier = multiplier.HasValue ? multiplier.Value : 1;
            this.Level = level.HasValue ? level.Value : 1;
        }

        [JsonIgnore]
        public BigDouble Total => (BaseValue + (ChangePerLevel * (Level - 1 ))) * Multiplier;

        public static implicit operator BigDouble(NumericAttribute attr) => attr.Total;
    }

    public class NumericAttributeConverter : JsonConverter<NumericAttribute>
    {
        public override NumericAttribute ReadJson(JsonReader reader, Type objectType, NumericAttribute existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            existingValue = new NumericAttribute(0);

            int loops = 0;

            while(true)
            {
                switch(reader.TokenType)
                {
                    case JsonToken.PropertyName:
                        switch (reader.Value)
                        {
                            case "$type":
                                reader.Read();
                                reader.Read();
                                continue;
                            default:
                                string name = (string)reader.Value;
                                var property = typeof(NumericAttribute).GetField(name);
                                if (property == null)
                                {
                                    throw new ArgumentException(reader.ReadAsString());
                                }
                                reader.Read(); // Consume the property token
                                property.SetValue(existingValue, serializer.Deserialize<BigDouble>(reader));
                                reader.Read(); // Consume closing end object
                                break;
                        }
                        break;
                    case JsonToken.EndObject:
                        return existingValue;
                    default:
                        reader.Read();
                        break;
                }
                
                if (loops > 100)
                {
                    throw new InvalidOperationException();
                }
                loops++;
            }
        }

        public override void WriteJson(JsonWriter writer, NumericAttribute value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanWrite => false;
    }
}