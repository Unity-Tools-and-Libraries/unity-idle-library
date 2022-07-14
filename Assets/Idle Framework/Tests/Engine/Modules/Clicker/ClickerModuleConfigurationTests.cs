using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker;
using MoonSharp.Interpreter;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace io.github.thisisnozaku.idle.framework.Tests.Engine.Modules.Clicker
{
    public class ClickerModuleConfigurationTests : TestsRequiringEngine
    {
        [SetUp]
        public void setup()
        {
            base.InitializeEngine();
            engine.AddModule(new ClickerModule());
        }

        [Test]
        public void SetsPointsProperties()
        {
            Assert.AreNotEqual(DataType.Nil, engine.Scripting.Evaluate("return points").Type);
            Assert.AreEqual(new BigDouble(0), engine.Scripting.Evaluate("return points.quantity").ToObject<BigDouble>());
            Assert.AreEqual(new BigDouble(0), engine.Scripting.Evaluate("return points.income").ToObject<BigDouble>());
            Assert.AreEqual(new BigDouble(1), engine.Scripting.Evaluate("return points.click_income").ToObject<BigDouble>());
        }

        [Test]
        public void DoClickIncreasesPointsByClickIncomeValue()
        {
            Assert.AreEqual(new BigDouble(0), engine.Scripting.Evaluate("return points.quantity").ToObject<BigDouble>());
            Assert.AreEqual(new BigDouble(1), engine.Scripting.Evaluate("return points.click_income").ToObject<BigDouble>());
            engine.Scripting.Evaluate("DoClick()");
            Assert.AreEqual(new BigDouble(1), engine.Scripting.Evaluate("return points.quantity").ToObject<BigDouble>());
        }

        [Test]
        public void AddsGlobalBuyProducerMethod()
        {
            Assert.AreNotEqual(DynValue.Nil, engine.Scripting.Evaluate("return BuyProducer"));
            Assert.AreEqual(DataType.ClrFunction, engine.Scripting.Evaluate("return BuyProducer").Type);
        }
    }
}