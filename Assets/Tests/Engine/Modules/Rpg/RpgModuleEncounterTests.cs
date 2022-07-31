using System.Collections;
using System.Collections.Generic;
using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg;
using NUnit.Framework;
using UnityEngine;

public class RpgModuleEncounterTests : RpgModuleTestsBase
{
    [Test]
    public void EncounterUpdateCallsUpdateOnCreatures()
    {
        random.SetNextValues(0);
        Configure();
        engine.Start();
        engine.StartEncounter();

        engine.SetActionPhase("combat");

        engine.Update(1f);

        Assert.AreEqual(new BigDouble(1), engine.GetPlayer<RpgCharacter>().ActionMeter);
        Assert.AreEqual(new BigDouble(1), engine.GetCurrentEncounter().Creatures[0].ActionMeter);
    }
}
