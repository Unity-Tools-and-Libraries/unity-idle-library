using io.github.thisisnozaku.idle.framework.Definitions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg
{
    public class CreatureDefinition : IDefinition
    {
        public string Id => throw new System.NotImplementedException();
        private IDictionary<string, object> properties;
        public IDictionary<string, object> Properties => properties;

        private CreatureDefinition(string id, IDictionary<string, object> properties) {
            this.properties = properties;
        }

        public class Builder
        {
            private IDictionary<string, object> properties = new Dictionary<string, object>();
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
        }
    }
}