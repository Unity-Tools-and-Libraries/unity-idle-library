using io.github.thisisnozaku.idle.framework.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg.Events
{
    /*
     * Event emitted when a character gains an ability.
     */
    public class AbilityAddedEvent : RpgCharacterModifierEvent<CharacterAbility>
    {
        public const string EventName = "abilityGained";
        private CharacterAbility ability;

        public AbilityAddedEvent(CharacterAbility ability)
        {
            this.ability = ability;
        }

        public override Dictionary<string, object> GetScriptingProperties()
        {
            return new Dictionary<string, object>()
            {
                { "ability",  ability}
            };
        }
    }

    /*
     * Event emitted when a character gains an ability.
     */
    public class AbilityRemovedEvent : RpgCharacterModifierEvent<CharacterAbility>
    {
        public const string EventName = "abilityGained";
        private CharacterAbility ability;

        public AbilityRemovedEvent(CharacterAbility ability)
        {
            this.ability = ability;
        }

        public override Dictionary<string, object> GetScriptingProperties()
        {
            return new Dictionary<string, object>()
            {
                { "ability",  ability}
            };
        }
    }
}