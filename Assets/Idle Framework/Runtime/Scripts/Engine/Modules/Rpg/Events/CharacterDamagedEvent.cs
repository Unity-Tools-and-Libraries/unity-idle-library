
using BreakInfinity;
using io.github.thisisnozaku.idle.framework;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg;
using io.github.thisisnozaku.idle.framework.Events;
using System;
using System.Collections.Generic;

public class CharacterDamagedEvent: ScriptingContext
{
    public const string EventName = "character_damaged";
    public static readonly List<Tuple<Type, String>> Arguments = new List<Tuple<Type, String>>()
        {
            Tuple.Create(typeof(ValueContainer), "The damaged character."),
            Tuple.Create(typeof(BigDouble), "The amount of damage inflicted.")
        };
    private Character damagedCharacter;
    private BigDouble damageInflicted;
    private Character attacker;

    public CharacterDamagedEvent(Character damagedCharacter, BigDouble damageInflicted, Character attacker)
    {
        this.damagedCharacter = damagedCharacter;
        this.damageInflicted = damageInflicted;
        this.attacker = attacker;
    }

    public Dictionary<string, object> GetScriptingContext(string contextType = null)
    {
        return new Dictionary<string, object>()
        {
            { "attacker", attacker },
            { "damage", damageInflicted },
            { "defender", damagedCharacter }
        };
    }
}
