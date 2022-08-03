
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg
{
    public class CreatureDefinition
    {
        public long Id { get; }

        public IDictionary<string, object> Properties { get; private set; } = new Dictionary<string, object>();

        [JsonConstructor]
        private CreatureDefinition(long id, IDictionary<string, object> properties) {
            this.Id = id;
            foreach(var property in properties)
            {
                this.Properties[property.Key] = property.Value;
            }
            
        }

        public class Builder
        {
            private IDictionary<string, object> properties = new Dictionary<string, object>()
            {
                { RpgCharacter.Attributes.DAMAGE,                      1 },
                { RpgCharacter.Attributes.MAXIMUM_HEALTH,              1 },
                { RpgCharacter.Attributes.ACCURACY,                    0 },
                { RpgCharacter.Attributes.ACTION,                      ""},
                { RpgCharacter.Attributes.ACTION_SPEED,                0 },
                { RpgCharacter.Attributes.PRECISION,                   0 },
                { RpgCharacter.Attributes.DEFENSE,                     0 },
                { RpgCharacter.Attributes.EVASION,                     0 },
                { RpgCharacter.Attributes.RESILIENCE,                  0 },
                { RpgCharacter.Attributes.PENETRATION,                 0 },
                { RpgCharacter.Attributes.CRITICAL_HIT_CHANCE,         0 },
                { RpgCharacter.Attributes.CRITICAL_DAMAGE_MULTIPLIER,  0 }
            };
            public CreatureDefinition Build(long withId)
            {
                return new CreatureDefinition(withId, properties);
            }

            public CreatureDefinition.Builder WithProperty(string property, object value)
            {
                properties[property] = value;
                return this;
            }
        }
    }
}