using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Engine.Persistence;
using NUnit.Framework;
using io.github.thisisnozaku.idle.framework.Engine;
using System;
using io.github.thisisnozaku.scripting.context;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg;
using JetBrains.Annotations;
using io.github.thisisnozaku.idle.framework.Engine.Achievements;
using MoonSharp.Interpreter;
using io.github.thisisnozaku.idle.framework.Events;

namespace io.github.thisisnozaku.idle.framework.Tests.Engine.Persistence
{
    public class EnginePersistenceTests : TestsRequiringEngine
    {
        [Test]
        public void CanSaveSimpleValuesInGlobalState()
        {
            engine.GlobalProperties["foo"] = 1;
            EngineSnapshot snapshot = engine.GetSnapshot();

            Assert.AreEqual(1, snapshot.Properties["foo"]);
        }

        [Test]
        public void CanSaveEntitiesInGlobalState()
        {
            TestEntity before = new TestEntity(engine, 1);
            before.ExtraProperties["extra"] = 5;
            before.SetFlag("flag");

            engine.GlobalProperties["foo"] = before;
            engine.GetProperty<TestEntity>("foo").Bar = new BigDouble(1);

            string snapshot = engine.GetSerializedSnapshotString();

            engine = new IdleEngine();

            engine.DeserializeSnapshotString(snapshot);

            TestEntity after = engine.GlobalProperties["foo"] as TestEntity;
            Assert.AreEqual(before.Id, after.Id);
            Assert.AreEqual(before.foo, after.foo);
            Assert.IsTrue(after.GetFlag("flag"));
            Assert.AreEqual(before.GetFlag("flag"), after.GetFlag("flag"));
            Assert.AreEqual(before.ExtraProperties["extra"], after.ExtraProperties["extra"]);

            //Assert.IsTrue(engine.Entities.ContainsKey(1));
            //Assert.AreEqual(1, engine.Entities.Count);
        }

        [Test]
        public void EventListenersSerialize()
        {
            engine.Watch("event", "test", "globals.triggered = true");

            var snapshot = engine.GetSerializedSnapshotString();

            engine = new IdleEngine();

            engine.DeserializeSnapshotString(snapshot);

            engine.Emit("event", (IScriptingContext)null);

            Assert.IsTrue(engine.GetProperty<bool>("triggered"));
        }

        [Test]
        public void EntityEventListenersSerialize()
        {
            engine.GlobalProperties["entity"] = new TestEntity(engine, 1);
            (engine.GlobalProperties["entity"] as TestEntity).Watch("event", "test", "globals.triggered = true");

            var snapshot = engine.GetSerializedSnapshotString();

            engine = new IdleEngine();

            engine.DeserializeSnapshotString(snapshot);

            var testEntity = engine.GlobalProperties["entity"] as TestEntity;

            testEntity.Emit("event", (IScriptingContext)null);

            Assert.IsTrue(engine.GetProperty<bool>("triggered"));
        }

        [Test]
        public void CallbackEventListenerDoNotSerialize()
        {
            engine.Watch("event", "test", MoonSharp.Interpreter.DynValue.FromObject(null, (System.Action)(() =>
            {

            })));

            var snapshot = engine.GetSerializedSnapshotString();

            Assert.IsNull(Newtonsoft.Json.Linq.JObject.Parse(snapshot).SelectToken("$.listeners.callbacks"));
        }

        [Test]
        public void CannotCallUpdateBeforeStart() // FIXME: Move into correct test suite.
        {
            Assert.Throws(typeof(InvalidOperationException), () => {
                engine.Update(1);
            });
        }

        [Test]
        public void AchievementStateSaved()
        {
            engine.Achievements[1L] = new framework.Engine.Achievements.Achievement(1, "", "return true");

            engine.Start();
            engine.Update(0f);

            Assert.IsTrue(engine.Achievements[1L].Completed);

            var snapshot = engine.GetSerializedSnapshotString();

            engine = new IdleEngine();

            engine.Achievements[1L] = new framework.Engine.Achievements.Achievement(1, "", "return true");

            Assert.IsFalse(engine.Achievements[1L].Completed);

            engine.DeserializeSnapshotString(snapshot);

            Assert.IsTrue(engine.Achievements[1].Completed);
        }

        [Test]
        public void NumericAttributeDeserializes()
        {
            TestEntity testEntity = new TestEntity(engine, 1);
            engine.GlobalProperties["entity"] = testEntity;
            testEntity.ExtraProperties["foo"] = new NumericAttribute(2);

            var serialized = engine.GetSerializedSnapshotString();

            engine = new IdleEngine();

            engine.DeserializeSnapshotString(serialized);

            Assert.AreEqual(new BigDouble(2),
                (engine.GetProperty<TestEntity>("entity").ExtraProperties["foo"] as NumericAttribute).ChangePerLevel);
        }

        [Test]
        public void AchievementsCanDeserialize()
        {
            engine.DefineAchievement(new Achievement(1L, "", "return true"));
            engine.Start();
            engine.Update(1f);

            Assert.IsTrue(engine.Achievements[1L].Completed);

            var serialized = engine.GetSerializedSnapshotString();

            engine = new IdleEngine();
            engine.DefineAchievement(new Achievement(1L, "", "return true"));

            Assert.IsFalse(engine.Achievements[1L].Completed);

            engine.DeserializeSnapshotString(serialized);

            Assert.IsTrue(engine.Achievements[1L].Completed);
        }

        [Test]
        public void AchievementsWithCallbackTriggerStatementSerializes()
        {
            engine.DefineAchievement(new Achievement(1L, "", "return true"));
            engine.Start();
            engine.Update(1f);

            Assert.IsTrue(engine.Achievements[1L].Completed);

            var serialized = engine.GetSerializedSnapshotString();

            engine = new IdleEngine();
            engine.DefineAchievement(new Achievement(1L, "", DynValue.FromObject(null, (Func<bool>)(() => true))));

            Assert.IsFalse(engine.Achievements[1L].Completed);

            engine.DeserializeSnapshotString(serialized);

            Assert.IsTrue(engine.Achievements[1L].Completed);
        }

        [Test]
        public void EmitReadyEventOnDeserialization()
        {
            engine.Watch(EngineReadyEvent.EventName, "", "globals.triggered = true");

            Assert.IsFalse(engine.GlobalProperties.ContainsKey("triggered"));

            engine.DeserializeSnapshotString(engine.GetSerializedSnapshotString());

            Assert.IsTrue((bool)engine.GlobalProperties["triggered"]);
        }
    }
}