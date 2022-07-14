using BreakInfinity;
namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg.Events
{
    public class AttackMissedEvent : AttackEvent
    {
        public const string EventName = "attack_missed";

        public AttackMissedEvent(RpgCharacter attacker, RpgCharacter defender, BigDouble attackDamage) : base(attacker, defender, attackDamage)
        {
        }
    }
}