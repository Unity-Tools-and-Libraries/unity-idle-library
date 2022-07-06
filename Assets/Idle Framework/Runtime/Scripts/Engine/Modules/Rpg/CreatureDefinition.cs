using io.github.thisisnozaku.idle.framework.Definitions;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg
{
    public class CreatureDefinition : IDefinition
    {
        public string Id { get; }
        private IDictionary<string, object> properties = new Dictionary<string, object>()
            {
                { Character.Attributes.DAMAGE,                      1 },
                { Character.Attributes.MAXIMUM_HEALTH,              1 },
                { Character.Attributes.ACCURACY,                    0 },
                { Character.Attributes.ACTION,                      ""},
                { Character.Attributes.ACTION_METER_SPEED,          0 },
                { Character.Attributes.DEFENSE,                     0 },
                { Character.Attributes.EVASION,                     0 },
                { Character.Attributes.PENETRATION,                 0 },
                { Character.Attributes.PRECISION,                   0 },
                { Character.Attributes.RESILIENCE,                  0 },
                { Character.Attributes.CRITICAL_HIT_CHANCE,         0 },
                { Character.Attributes.CRITICAL_DAMAGE_MULTIPLIER,  0 }
            };
        public IDictionary<string, object> Properties => properties;

        [JsonConstructor]
        private CreatureDefinition(string id, IDictionary<string, object> properties) {
            this.Id = id;
            foreach(var property in properties)
            {
                this.properties[property.Key] = property.Value;
            }
            
        }

        public class Builder
        {
            private IDictionary<string, object> properties = new Dictionary<string, object>()
            {
                { Character.Attributes.DAMAGE,                      1 },
                { Character.Attributes.MAXIMUM_HEALTH,              1 },
                { Character.Attributes.ACCURACY,                    0 },
                { Character.Attributes.ACTION,                      ""},
                { Character.Attributes.ACTION_METER_SPEED,          0 },
                { Character.Attributes.DEFENSE,                     0 },
                { Character.Attributes.EVASION,                     0 },
                { Character.Attributes.PENETRATION,                 0 },
                { Character.Attributes.CRITICAL_HIT_CHANCE,         0 },
                { Character.Attributes.CRITICAL_DAMAGE_MULTIPLIER,  0 }
            };
            public CreatureDefinition Build(string withId)
            {
                return new CreatureDefinition(withId, properties);
            }

            public Builder WithHealthExpression(string expression)
            {
                properties[Character.Attributes.MAXIMUM_HEALTH] = expression;
                return this;
            }

            public Builder WithDamageExpression(string expression)
            {
                properties[Character.Attributes.DAMAGE] = expression;
                return this;
            }

            public Builder WithIcon(string iconPath)
            {
                properties[Character.Attributes.ICON] = iconPath;
                return this;
            }
        }
    }
}