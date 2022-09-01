using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Engine.Persistence;
using NUnit.Framework;
using io.github.thisisnozaku.idle.framework.Engine;
using io.github.thisisnozaku.idle.framework.Events;
using System;
using io.github.thisisnozaku.scripting.context;

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

            Assert.IsTrue(engine.Entities.ContainsKey(1));
            Assert.AreEqual(1, engine.Entities.Count);
        }

        [Test]
        public void EventListenersSerialize()
        {
            engine.Watch("event", "test", "triggered = true");

            var snapshot = engine.GetSerializedSnapshotString();

            engine = new IdleEngine();

            engine.DeserializeSnapshotString(snapshot);

            engine.Emit("event", (IScriptingContext)null);

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
        public void SavesUsedEntityIds()
        {
            
        }
    }
}