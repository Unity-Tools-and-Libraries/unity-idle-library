using BreakInfinity;
using System;
using System.Collections.Generic;

namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg
{
    public abstract class AttackEvent : CombatInteractionEvent
    {
        private BigDouble attackDamage;

        protected AttackEvent(RpgCharacter attacker, RpgCharacter defender, BigDouble attackDamage): base(attacker, defender)
        {
            if(attacker == null)
            {
                throw new ArgumentNullException("attacker");
            }
            if(defender == null)
            {
                throw new ArgumentNullException("defender");
            }
            this.attackDamage = attackDamage;
        }

        public override Dictionary<string, object> GetScriptingProperties()
        {
            return new Dictionary<string, object>()
            {
                { "attacker", source },
                { "defender", target },
                { "damage", attackDamage }
            };
        }
    }
}