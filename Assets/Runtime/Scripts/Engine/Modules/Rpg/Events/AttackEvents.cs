using BreakInfinity;
using System;
using System.Collections.Generic;

namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg
{
    /*
     * Abstract base class for attack events.
     */
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
    /*
     * Event emitted by a character when the character makes an attack that misses.
     */
    public class AttackMissedEvent : AttackEvent
    {
        public const string EventName = "attack_failed";

        public AttackMissedEvent(RpgCharacter attacker, RpgCharacter defender, BigDouble attackDamage) : base(attacker, defender, attackDamage)
        {
        }
    }
    /*
     * Event emitted by a character when the character is targeted by an attack and missed.
     */
    public class MissedByAttackEvent : AttackEvent
    {
        public const string EventName = "attack_missed";

        public MissedByAttackEvent(RpgCharacter attacker, RpgCharacter defender, BigDouble attackDamage) : base(attacker, defender, attackDamage)
        {
        }
    }
    /*
     * Event emitted by a character when the character makes an attack that hits.
     */
    public class AttackHitEvent : AttackEvent
    {
        public const string EventName = "attack_hit";

        public AttackHitEvent(RpgCharacter attacker, RpgCharacter defender, BigDouble attackDamage) : base(attacker, defender, attackDamage)
        {
        }
    }

    public class HitByAttackEvent: AttackEvent
    {
        public const string EventName = "attack_hit";

        public HitByAttackEvent(RpgCharacter attacker, RpgCharacter defender, BigDouble attackDamage) : base(attacker, defender, attackDamage)
        {
        }
    } 
}