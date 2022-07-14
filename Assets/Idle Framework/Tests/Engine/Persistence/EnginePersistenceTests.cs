using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Engine.Persistence;
using NUnit.Framework;
using io.github.thisisnozaku.idle.framework.Tests.Engine.Scripting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using io.github.thisisnozaku.idle.framework.Engine;
using io.github.thisisnozaku.idle.framework.Events;
using System;

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
            engine.GlobalProperties["foo"] = new TestCustomType(engine);
            engine.GetProperty<TestCustomType>("foo").Bar = new BigDouble(1);
            
            var settings = new JsonSerializerSettings();
            settings.TypeNameHandling = TypeNameHandling.All;

            string snapshot = JsonConvert.SerializeObject(engine.GetSnapshot(), settings);

            var loadSettings = new JsonLoadSettings();

            var before = engine.GlobalProperties["foo"];

            engine.RestoreFromSnapshot(JsonConvert.DeserializeObject<EngineSnapshot>(snapshot, settings));

            Assert.AreEqual(before, engine.GlobalProperties["foo"]);
        }

        [Test]
        public void EventListenersSerialize()
        {
            engine.Watch("event", "test", "triggered = true");

            var snapshot = engine.GetSerializedSnapshotString();

            engine = new IdleEngine();

            engine.DeserializeSnapshotString(snapshot);

            engine.Emit("event", (ScriptingContext)null);

            Assert.IsTrue(engine.GetProperty<bool>("triggered"));
        }

        [Test]
        public void CannotCallUpdateBeforeStart()
        {
            Assert.Throws(typeof(InvalidOperationException), () => {
                engine.Update(1);
            });
        }
    }
}