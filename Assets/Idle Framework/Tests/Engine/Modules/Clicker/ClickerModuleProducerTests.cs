using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker;
using MoonSharp.Interpreter;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace io.github.thisisnozaku.idle.framework.Tests.Engine.Modules.Clicker {
    public class ClickerModuleProducerTests : TestsRequiringEngine
    {
        [SetUp]
        public void setup()
        {
            base.InitializeEngine();
            var module = new ClickerModule();

            module.AddProducer(new Producer(engine, "one", "one", 1, 1));

            engine.AddModule(module);
        }

        [Test]
        public void BuyingProducerIncreasesQuantity()
        {
            Assert.AreEqual(new BigDouble(0), engine.Scripting.Evaluate("return producers.one.quantity").ToObject<BigDouble>());
            engine.Scripting.Evaluate("BuyProducer('one')");
            Assert.AreEqual(new BigDouble(1), engine.Scripting.Evaluate("return producers.one.quantity").ToObject<BigDouble>());
        }

        [Test]
        public void TotalIncomeEqualToQuantityTimePerUnitIncome()
        {
            engine.Start();
            engine.GetProducers()["one"].Quantity = 1;
            engine.Update(0f);
            Assert.AreEqual(new BigDouble(1), engine.Scripting.Evaluate("return producers.one.TotalOutput").ToObject<BigDouble>());
        }
    }
}