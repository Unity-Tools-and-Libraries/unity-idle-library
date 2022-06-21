using io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace io.github.thisisnozaku.idle.framework.Tests.Engine.Modules.Rpg.Statuses
{
    public class RpgModuleStatusDefinitionTests : RequiresEngineTests
    {
        [Test]
        public void CanDefineAPropertySetter()
        {
            var def = new StatusDefinition.Builder()
                .SetsProperty("property", "true")
                .Build("", "");
            Assert.AreEqual("=true", def.Modifications["target.property"]);
        }

        [Test]
        public void CanDefineAnAttributeModifier()
        {
            var def = new StatusDefinition.Builder()
                .FlatAttributeBonus("accuracy", 1)
                .Build("", "");
            Assert.AreEqual("+1", def.Modifications["target.accuracy"]);
            def = new StatusDefinition.Builder()
                .FlatAttributeBonus("accuracy", -1)
                .Build("", "");
            Assert.AreEqual("+-1", def.Modifications["target.accuracy"]);
        }

        [Test]
        public void CanDefineAnAttributeModifierWithAnExpression()
        {
            var def = new StatusDefinition.Builder()
                .FlatAttributeBonus("accuracy", "1 + 2")
                .Build("", "");
            Assert.AreEqual("+1 + 2", def.Modifications["target.accuracy"]);
        }
    }
}