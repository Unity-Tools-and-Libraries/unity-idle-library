using System;
using System.Collections.Generic;

namespace IdleFramework
{
    /*
     * Defines the properties of a Modifier.
     */
    public class ModifierDefinition : ModifierDefinitionProperties
    {
        private string modifierKey;
        private StateMatcher trigger;
        private ISet<EntityEffect> entityEffects = new HashSet<EntityEffect>();

        public string ModifierKey => modifierKey;
        public StateMatcher Trigger => trigger;
        public IEnumerable<EntityEffect> Effects => entityEffects;

        public ModifierDefinition(string modifierKey, StateMatcher trigger, ISet<EntityEffect> entityEffects)
        {
            this.modifierKey = modifierKey;
            this.trigger = trigger;
            this.entityEffects = entityEffects;            
        }

        public bool IsActive(IdleEngine engine)
        {
            return trigger.Matches(engine);
        }

        public override bool Equals(object obj)
        {
            if(!typeof(ModifierDefinition).Equals(obj.GetType()))
            {
                return false;
            }
            ModifierDefinition other = (ModifierDefinition) obj;
            return this.modifierKey == other.ModifierKey && this.trigger.Equals(other.trigger) &&
                this.entityEffects.Equals(other.entityEffects);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("Modifier({0})");
        }
    }

    public interface ModifierDefinitionProperties
    {
        string ModifierKey { get; }
    }
}