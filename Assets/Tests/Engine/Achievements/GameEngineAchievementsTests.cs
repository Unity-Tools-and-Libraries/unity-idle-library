using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Engine.Achievements;
using io.github.thisisnozaku.idle.framework.Engine.Achievements.Events;
using MoonSharp.Interpreter;
using NUnit.Framework;
using System;
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
            engine.DefineAchievement(new Achievement(1L, "", "return globals.foobar.baz == 1"));

            engine.GlobalProperties["foobar"] = new Dictionary<string, object>()
            {
                { "baz", BigDouble.One }
            };

            engine.Start();

            engine.Update(0f);

            Assert.IsTrue(engine.Achievements[1L].Completed);
        }

        [Test]
        public void AchievementDoesNotCompleteWhenRequirementNotMet()
        {
            engine.DefineAchievement(new Achievement(1L, "", "return globals.foobar.baz == 1"));

            engine.GlobalProperties["foobar"] = new Dictionary<string, object>()
            {
                { "baz", BigDouble.Zero }
            };

            bool called = false;

            engine.Watch(AchievementCompletedEvent.EventName, "", DynValue.FromObject(null, (Action)(() =>
            {
                called = true;
            })));

            engine.Start();

            engine.Update(0f);

            Assert.IsFalse(called);
        }

        [Test]
        public void CompletedAchievementDoesNotBecomeUncompleteWhenNoLongerTrue()
        {
            engine.DefineAchievement(new Achievement(1L, "", "return globals.foobar.baz == 1"));

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
            engine.DefineAchievement(new Achievement(1L, "", "return globals.foobar.baz == 1"));

            engine.GlobalProperties["foobar"] = new Dictionary<string, object>()
            {
                { "baz", BigDouble.One }
            };

            engine.Start();

            engine.Watch(AchievementCompletedEvent.EventName, "", "globals.called = true");

            engine.Update(0f);

            Assert.IsTrue((bool?)engine.GlobalProperties["called"]);
        }

        [Test]
        public void CompletionCanHaveEffects()
        {
            engine.DefineAchievement(new Achievement(1L, "return globals.foobar.baz == 1", "globals.foobarbaz = 1"));

            engine.GlobalProperties["foobar"] = new Dictionary<string, object>()
            {
                { "baz", BigDouble.One }
            };

            engine.Start();

            engine.Update(0f);

            Assert.AreEqual(BigDouble.One, engine.GlobalProperties["foobarbaz"]);
        }

        [Test]
        public void OnDeserializationDoesNotResetDefinition()
        {
            engine.DefineAchievement(new Achievement(1L, "", "return globals.foobar.baz == 1"));

            engine.GlobalProperties["foobar"] = new Dictionary<string, object>()
            {
                { "baz", BigDouble.One }
            };

            engine.DeserializeSnapshotString(engine.GetSerializedSnapshotString());

            Assert.IsNull(engine.Achievements[1L].CompletionEffect);
        }
    }
}