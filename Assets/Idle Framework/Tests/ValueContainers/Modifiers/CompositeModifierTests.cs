using BreakInfinity;
using io.github.thisisnozaku.idle.framework;
using io.github.thisisnozaku.idle.framework.Events;
using io.github.thisisnozaku.idle.framework.Modifiers;
using NUnit.Framework;
using System.Collections.Generic;
namespace io.github.thisisnozaku.idle.framework.Tests.Modifiers
{
    public class CompositeModifierTests : RequiresEngineTests
    {
        [Test]
        public void OnAddAppliesChildModifications()
        {
            var modifier = new MockCompositeListener("", "", Modifications: new Dictionary<string, string>()
        {
            { "childString", "=\"foobar\"" },
            { "childNumber", "=5" }
        });
            var target = engine.SetProperty("path", new Dictionary<string, ValueContainer>());
            target.AddModifier(modifier);
            Assert.AreEqual("foobar", target.GetProperty("childString").ValueAsString());
            Assert.AreEqual(new BigDouble(5), target.GetProperty("childNumber").ValueAsNumber());
        }

        [Test]
        public void OnRemoveUndoesOnAddChanged()
        {
            var modifier = new MockCompositeListener("", "", Modifications: new Dictionary<string, string>()
        {
            { "childString", "=\"foobar\"" },
            { "childNumber", "=5" }
        });
            var target = engine.SetProperty("path", new Dictionary<string, ValueContainer>());
            target.AddModifier(modifier);
            Assert.AreEqual("foobar", target.GetProperty("childString").ValueAsString());
            Assert.AreEqual(new BigDouble(5), target.GetProperty("childNumber").ValueAsNumber());
            target.RemoveModifier(modifier);
            Assert.Null(target.GetProperty("childString").ValueAsRaw());
            Assert.Null(target.GetProperty("childNumber").ValueAsRaw());
        }

        [Test]
        public void CanTriggerEventsOnModifiers()
        {
            engine.Start();
            int callCount = 0;
            var modifier = new MockCompositeListener("", "", Events: new Dictionary<string, List<string>>()
            {
                { "event", new List<string>() { "method()" } }
            });
            var target = engine.SetProperty("path", new Dictionary<string, ValueContainer>());
            target.AddModifier(modifier);
            engine.RegisterMethod("method", (a, b, c) => {
                callCount++;
                return null;
            });
            target.NotifyImmediately("event", new ValueChangedEvent("", "", null, null));
            Assert.AreEqual(new BigDouble(1), new BigDouble(callCount));
        }

        public class MockCompositeListener : CompositeModifier
        {
            public MockCompositeListener(string id, string description, Dictionary<string, string> Modifications = null, Dictionary<string, List<string>> Events = null, int priority = 0) : base(id, description, Modifications, Events, priority)
            {
            }
        }
    }
}