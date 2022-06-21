using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Definitions;
using io.github.thisisnozaku.idle.framework.Modifiers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg
{
    /*
     * An abstract base class for something that applies a change to a character.
     */
    public abstract class CharacterModifierDefinition : CompositeModifier, IDefinition
    {
        private IDictionary<string, object> properties;
        public CharacterModifierDefinition(string id, string description, IDictionary<string, object> properties, IDictionary<string, string> modifications, IDictionary<string, List<string>> events = null) : base(id, description, modifications, events)
        {
            this.properties = properties;
        }

        IDictionary<string, object> IDefinition.Properties => this.properties;

        public override bool CanApply(object target)
        {
            return target.GetType() == typeof(Character);
        }

        public abstract class Builder<T>
        {
            protected IDictionary<string, string> Modifications = new Dictionary<string, string>();
            /*
             * Have the status add to the given attribute the specified amount.
             */
            public Builder<T> FlatAttributeBonus(string attribute, BigDouble value)
            {
                return FlatAttributeBonus(attribute, value.ToString());
            }

            public Builder<T> FlatAttributeBonus(string attribute, string expression)
            {
                Modifications.Add("target." + attribute, "+" + expression);
                return this;
            }

            public Builder<T> SetsProperty(string property, string expression)
            {
                Modifications.Add("target." + property, "=" + expression);
                return this;
            }

            public abstract T Build(string id, string description);

        }
    }
}