using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker.Definitions.Upgrades;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace io.github.thisisnozaku.idle.framework.Tests.Engine.Modules.Clicker
{
    public class ProducerTests : RequiresEngineTests
    {
        ClickerModule module;
        ProducerDefinition tier1;
        ProducerDefinition tier2;
        ProducerDefinition tier3;

        [SetUp]
        public void Setup()
        {
            module = new ClickerModule();
            tier1 = new ProducerDefinition("one", "one", 15, 0.1);
            tier2 = new ProducerDefinition("two", "two", 100, 1, enableExpression: "producers.one.quantity > 0");
            tier3 = new ProducerDefinition("three", "three", 1100, 5, unlockExpression: "producers.one.quantity > 0", enableExpression: "producers.two.quantity > 0");
            module.AddProducer(tier1);
            module.AddProducer(tier2);
            module.AddProducer(tier3);

            module.AddUpgrade(new FlatProducerUpgradeDefinition("upgrade-one", "upgrade one", 100, "true", "true", Tuple.Create("producers.one.output_per_unit_per_second", 0.1.ToString())));
            module.AddUpgrade(new MultiplierProducerUpgradeDefinition("upgrade-two", "upgrade two", 100, "true", "true", Tuple.Create("producers.two.output_per_unit_per_second", 1.5.ToString())));

            engine.AddModule(module);

            engine.Start();
        }

        [Test]
        public void DuplicateProducerIdThrowsException()
        {
            Assert.Throws(typeof(ArgumentException), () =>
            {
                module.AddProducer(new ProducerDefinition("one", "", 1, 0));
            });
        }

        [Test]
        public void CostIncreasesBasedOnQuantity()
        {
            engine.ConfigureLogging("engine.internal.container.cache", LogType.Log);
            var producerQuantity = engine.CreateProperty("producers.one.quantity");
            for (int i = 0; i < 10; i++) {
                producerQuantity.Set(i);
                engine.Update(0f);
                Assert.AreEqual(new BigDouble(15) * new BigDouble(1.15).Pow(i), engine.GetProperty("producers.one.cost").ValueAsNumber());
            }
        }

        [Test]
        public void QuantityChangesOnUpdate()
        {
            engine.GetProperty("producers.one.quantity").Set(1);
            Assert.AreEqual(new BigDouble(.1), engine.GetProperty("points.income").ValueAsNumber());
            engine.Update(1);
            Assert.AreEqual(new BigDouble(.1), engine.GetProperty("points.quantity").ValueAsNumber());
        }

        [Test]
        public void UpgradeCanIncreaseOutputByAFlatAmount()
        {
            engine.InvokeMethod(ClickerModule.BuyUpgrade, null, "upgrade-one");
            Assert.AreEqual(new BigDouble(0.2), engine.GetProperty(string.Join(".", "producers", tier1.Id, ProducerDefinition.PropertyNames.OUTPUT_PER_UNIT)).ValueAsNumber());
        }

        [Test]
        public void UpgradeCanIncreaseOutputByAMultiplier()
        {
            engine.InvokeMethod(ClickerModule.BuyUpgrade, null, "upgrade-two");
            Assert.AreEqual(new BigDouble(1.5), engine.GetProperty(string.Join(".", "producers", tier2.Id, ProducerDefinition.PropertyNames.OUTPUT_PER_UNIT)).ValueAsNumber());
        }

        [Test]
        public void BuyingProducerSubtractsPoints()
        {
            engine.GetProperty("points.quantity").Set(15);
            engine.InvokeMethod(ClickerModule.BuyProducer, null, "one");
            Assert.AreEqual(new BigDouble(0), engine.GetProperty("points.quantity").ValueAsNumber());
        }

        [Test]
        public void ProducerUpdaterSetsEnabledFlagIfExpressionTrue()
        {
            engine.GetProperty("producers.one.quantity").Set(1);
            engine.Update(1);
            Assert.True(engine.GetProperty("producers.two.enabled").ValueAsBool());
        }

        [Test]
        public void ProducerUpdaterSetsUnlockedFlagIfExpressionTrue()
        {
            engine.GetProperty("producers.one.quantity").Set(1);
            engine.Update(1);
            Assert.True(engine.GetProperty("producers.three.unlocked").ValueAsBool());
        }
    }
}