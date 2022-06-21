using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace io.github.thisisnozaku.idle.framework.Tests.Engine
{
    public class PathTraversalTests : RequiresEngineTests
    {
        [Test]
        public void CaretGoesUpOneLevel()
        {
            var container = engine.CreateProperty("top.middle.bottom");
            Assert.AreEqual("top.middle", container.GetProperty("^").Path);
            Assert.AreEqual("top.middle", engine.GetProperty("top.middle.bottom.^").Path);
        }

        [Test]
        public void CaretAtTopLevelThrows()
        {
            var container = engine.CreateProperty("top");
            Assert.Throws(typeof(InvalidOperationException), () => {
                container.GetProperty("^");
            });
        }

        [Test]
        public void NavigatingToNonExistantChildPathWithGetOrCreateOptionReturnsNewContainer()
        {
            var container = engine.CreateProperty("top");
            
            Assert.NotNull(engine.GetProperty("top.middle", framework.Engine.IdleEngine.GetOperationType.GET_OR_CREATE));
        }

        [Test]
        public void NavigatingToNonExistantChildPathWithGetOrNullOptionReturnsNull()
        {
            var container = engine.CreateProperty("top");

            Assert.Null(engine.GetProperty("top.middle", framework.Engine.IdleEngine.GetOperationType.GET_OR_NULL));
            Assert.Null(engine.GetProperty("top.middle"));
        }

        [Test]
        public void NavigatingToNonExistantChildPathWithGetOrThrowOptionThrows()
        {
            var container = engine.CreateProperty("top");

            Assert.Throws(typeof(InvalidOperationException), () =>
            {
                Assert.Null(engine.GetProperty("top.middle", framework.Engine.IdleEngine.GetOperationType.GET_OR_THROW));
            });
        }
    }
}