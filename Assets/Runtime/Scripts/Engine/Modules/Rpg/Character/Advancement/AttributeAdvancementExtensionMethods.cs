using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg.Configuration;
using io.github.thisisnozaku.scripting.context;
using UnityEngine;

public static class AttributeAdvancementExtensionMethods
{
    public static Dictionary<string, BigDouble> GetCostToBuyAttribute(this RpgCharacter character, string attribute)
    {
        var engine = character.Engine;

        var configuration = engine.GetConfiguration<CharacterAdvancementConfiguration>("player.Advancement");

        var calculators = configuration.GetAttributePurchaseCalculator(attribute);

        return calculators.ToDictionary(x => x.Key, x => engine.Scripting.Evaluate(x.Value,
            Tuple.Create<string, object>("level", character.GetAttribute(attribute).Level + 1), new List<string>()
            {
                "level"
            }).ToObject<BigDouble>());
    }
}
