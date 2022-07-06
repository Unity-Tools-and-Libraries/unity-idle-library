using BreakInfinity;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace io.github.thisisnozaku.idle.framework.Tests.Engine
{
    public class IdleEngineSnapshotTests : RequiresEngineTests
    {
        [Test]
        public void GetsAllGlobalProperties()
        {
            engine.CreateProperty("foo", 1);
            engine.CreateProperty("bar", 2);
            engine.CreateProperty("baz", 3);
            var saved = engine.GetSnapshot();
            Assert.AreEqual(new BigDouble(1), saved.GlobalProperties["foo"].Value);
            Assert.AreEqual(new BigDouble(2), saved.GlobalProperties["bar"].Value);
            Assert.AreEqual(new BigDouble(3), saved.GlobalProperties["baz"].Value);

            Assert.AreEqual("foo", saved.GlobalProperties["foo"].Path);
            Assert.AreEqual("bar", saved.GlobalProperties["bar"].Path);
            Assert.AreEqual("baz", saved.GlobalProperties["baz"].Path);
        }

        [Test]
        public void SavesListeners()
        {
            engine.Subscribe("", "event", "method");
            var saved = engine.GetSnapshot();

            Assert.AreEqual(new ValueContainer.ListenerSubscription("", "", "event", "method", false), saved.Listeners[0]);
        }

        [Test]
        public void DoesNotSaveEphemeralListeners()
        {
            engine.Subscribe("", "event", "method", ephemeral: true);
            var saved = engine.GetSnapshot();

            Assert.AreEqual(0, saved.Listeners.Count);
        }

        [Test]
        public void RestoresContainersFromSnapshot()
        {
            engine.RestoreFromSnapshot(new framework.Engine.IdleEngine.Snapshot(new Dictionary<string, ValueContainer.Snapshot>()
            {
                { "foo", new ValueContainer.Snapshot("foo", new BigDouble(1), null) },
                { "bar", new ValueContainer.Snapshot("bar", new BigDouble(2), null) },
                { "baz", new ValueContainer.Snapshot("baz", new BigDouble(3), null) }
            }, new List<ValueContainer.ListenerSubscription>()));
            Assert.AreEqual(new BigDouble(1), engine.GetProperty("foo").ValueAsRaw());
            Assert.AreEqual(new BigDouble(2), engine.GetProperty("bar").ValueAsRaw());
            Assert.AreEqual(new BigDouble(3), engine.GetProperty("baz").ValueAsRaw());
        }

        [Test]
        public void RestoresListenersFromSnapshot()
        {
            bool listenerCalled = false;
            engine.RegisterMethod("method", (ie, args) => {
                listenerCalled = true;
                return null;
            });
            engine.NotifyImmediately("event");
            Assert.False(listenerCalled);
            engine.RestoreFromSnapshot(new framework.Engine.IdleEngine.Snapshot(new Dictionary<string, ValueContainer.Snapshot>(), new List<ValueContainer.ListenerSubscription>()
            {
                new ValueContainer.ListenerSubscription("", "", "event", "method()", false)
            }));
            engine.NotifyImmediately("event");
            Assert.True(listenerCalled);
        }
    }
}