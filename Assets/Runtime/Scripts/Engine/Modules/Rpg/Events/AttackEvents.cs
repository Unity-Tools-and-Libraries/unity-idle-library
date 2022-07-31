using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg.Combat;
using System;
using System.Collections.Generic;

namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg
{
    /*
     * Abstract base class for attack events.
     */
    public abstract class AttackEvent : CombatInteractionEvent
    {
        private AttackResultDescription attackResultDescription;

        protected AttackEvent(RpgCharacter attacker, RpgCharacter defender, AttackResultDescription attackResultDescription): base(attacker, defender)
        {
            if(attacker == null)
            {
                throw new ArgumentNullException("attacker");
            }
            if(defender == null)
            {
                throw new ArgumentNullException("defender");
            }
        }

        public override Dictionary<string, object> GetScriptingProperties()
        {
            return new Dictionary<string, object>()
            {
                { "attacker", source },
                { "defender", target },
                { "attack", attackResultDescription }
            };
        }
    }
    /*
     * Event emitted by a character when the character makes an attack that misses.
     */
    public class AttackMissedEvent : AttackEvent
    {
        public const string EventName = "attack_failed";

        public AttackMissedEvent(RpgCharacter attacker, RpgCharacter defender, AttackResultDescription attackResultDescription) : base(attacker, defender, attackResultDescription)
        {
        }
    }
    /*
     * Event emitted by a character when the character is targeted by an attack and missed.
     */
    public class MissedByAttackEvent : AttackEvent
    {
        public const string EventName = "attack_missed";

        public MissedByAttackEvent(RpgCharacter attacker, RpgCharacter defender, AttackResultDescription attackResultDescription) : base(attacker, defender, attackResultDescription)
        {
        }
    }
    /*
     * Event emitted by a character when the character makes an attack that hits.
     */
    public class AttackHitEvent : AttackEvent
    {
        public const string EventName = "attack_hit";

        public AttackHitEvent(RpgCharacter attacker, RpgCharacter defender, AttackResultDescription attackResultDescription) : base(attacker, defender, attackResultDescription)
        {
        }
    }

    public class HitByAttackEvent: AttackEvent
    {
        public const string EventName = "attack_hit";

        public HitByAttackEvent(RpgCharacter attacker, RpgCharacter defender, AttackResultDescription attackResultDescription) : base(attacker, defender, attackResultDescription)
        {
        }
    } 
}