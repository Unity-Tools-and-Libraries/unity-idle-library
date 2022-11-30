using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Engine.Achievements;
using io.github.thisisnozaku.idle.framework.Engine.Achievements.Events;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace io.github.thisisnozaku.idle.framework.Tests.Engine.Achievements
{
    public class GameEngineAchievementsTests : TestsRequiringEngine
    {
        [Test]
        public void CanCompleteAchievementWhenAttributeHasValue()
        {
            engine.DefineAchievement(new Achievement(1L, "return foobar.baz == 1"));

            engine.GlobalProperties["foobar"] = new Dictionary<string, object>()
            {
                { "baz", BigDouble.One }
            };

            engine.Start();

            engine.Update(0f);

            Assert.IsTrue(engine.Achievements[1L].Completed);
        }

        [Test]
        public void CompletedAchievementDoesNotBecomeUncompleteWhenNoLongerTrue()
        {
            engine.DefineAchievement(new Achievement(1L, "return foobar.baz == 1"));

            engine.GlobalProperties["foobar"] = new Dictionary<string, object>()
            {
                { "baz", BigDouble.One }
            };

            engine.Start();

            engine.Update(0f);

            Assert.IsTrue(engine.Achievements[1L].Completed);

            (engine.GlobalProperties["foobar"] as Dictionary<string, object>)["baz"] = BigDouble.Zero;

            engine.Update(0f);

            Assert.IsTrue(engine.Achievements[1L].Completed);
        }

        [Test]
        public void CompletionEmitsEvent()
        {
            engine.DefineAchievement(new Achievement(1L, "return foobar.baz == 1"));

            engine.GlobalProperties["foobar"] = new Dictionary<string, object>()
            {
                { "baz", BigDouble.One }
            };

            engine.Start();

            engine.Watch(AchievementCompletedEvent.EventName, "", "called = true");

            engine.Update(0f);

            Assert.IsTrue((bool?)engine.GlobalProperties["called"]);
        }

        [Test]
        public void ComplationCanHaveEffects()
        {
            engine.DefineAchievement(new Achievement(1L, "return foobar.baz == 1", "foobarbaz = 1"));

            engine.GlobalProperties["foobar"] = new Dictionary<string, object>()
            {
                { "baz", BigDouble.One }
            };

            engine.Start();

            engine.Update(0f);

            Assert.AreEqual(BigDouble.One, engine.GlobalProperties["foobarbaz"]);
        }
    }
}