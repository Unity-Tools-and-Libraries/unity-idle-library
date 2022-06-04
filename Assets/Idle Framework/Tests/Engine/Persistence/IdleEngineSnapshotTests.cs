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
            engine.SetProperty("foo", 1);
            engine.SetProperty("bar", 2);
            engine.SetProperty("baz", 3);
            var saved = engine.GetSnapshot();
            Assert.AreEqual(new BigDouble(1), saved.GlobalProperties["foo"].Value);
            Assert.AreEqual(new BigDouble(2), saved.GlobalProperties["bar"].Value);
            Assert.AreEqual(new BigDouble(3), saved.GlobalProperties["baz"].Value);

            Assert.AreEqual("foo", saved.GlobalProperties["foo"].Path);
            Assert.AreEqual("bar", saved.GlobalProperties["bar"].Path);
            Assert.AreEqual("baz", saved.GlobalProperties["baz"].Path);
        }
    }
}