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
            var container = engine.SetProperty("top.middle.bottom");
            Assert.AreEqual("top.middle", container.GetProperty("^").Path);
            Assert.AreEqual("top.middle", engine.GetProperty("top.middle.bottom.^").Path);
        }

        [Test]
        public void CaretAtTopLevelThrows()
        {
            var container = engine.SetProperty("top");
            Assert.Throws(typeof(InvalidOperationException), () => {
                container.GetProperty("^");
            });
        }

        [Test]
        public void NavigatingToNonExistantChildPathReturnsNull()
        {
            var container = engine.SetProperty("top");
            
            Assert.IsNull(engine.GetProperty("top.middle"));
        }

        [Test]
        public void NavigatingToNonExistantGlobalPathReturnsNull()
        {
            Assert.IsNull(engine.GetProperty("top.middle"));
        }
    }
}