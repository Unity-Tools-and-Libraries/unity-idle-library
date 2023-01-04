using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace io.github.thisisnozaku.idle.framework.Tests.Engine.Modules.Clicker
{
    public class ClickerProducerCommandTests : ClickerModuleTestsBase
    {
        [SetUp]
        public void Setup()
        {
            Configure();
            engine.State.Transition(ClickerModule.States.GAMEPLAY);
        }

        [Test]
        public void GainProducerCommandMustHaveProducerName()
        {
            var thrown = Assert.Throws(typeof(InvalidOperationException), () =>
            {
                engine.EvaluateCommand("gainProducer");
            });
            Assert.AreEqual("Need producer id in position 1. (Usage: gainProducer [producerId] [quantity])", thrown.Message);
        }

        [Test]
        public void GainProducerCommandMustHaveQuantity()
        {
            var thrown = Assert.Throws(typeof(InvalidOperationException), () =>
            {
                engine.EvaluateCommand("gainProducer Producer");
            });
            Assert.AreEqual("Need producer quantity in position 2. (Usage: gainProducer [producerId] [quantity])", thrown.Message);
        }

        [Test]
        public void GainProducerCommandIncreasesProducer()
        {
            Assert.AreEqual(BigDouble.Zero, engine.GetProperty<BigDouble>("player.Producers[1].Quantity"));
            engine.EvaluateCommand("gainProducer 1 1");
            Assert.AreEqual(BigDouble.One, engine.GetProperty<BigDouble>("player.Producers[1].Quantity"));
        }

        [Test]
        public void buyProducerCommandMustHaveProducerName()
        {
            var thrown = Assert.Throws(typeof(InvalidOperationException), () =>
            {
                engine.EvaluateCommand("buyProducer");
            });
            Assert.AreEqual("Need producer id in position 1. (Usage: buyProducer [producerId] [quantity])", thrown.Message);
        }

        [Test]
        public void buyProducerCommandMustHaveQuantity()
        {
            var thrown = Assert.Throws(typeof(InvalidOperationException), () =>
            {
                engine.EvaluateCommand("buyProducer Producer");
            });
            Assert.AreEqual("Need producer quantity in position 2. (Usage: buyProducer [producerId] [quantity])", thrown.Message);
        }

        [Test]
        public void buyProducerCommandReducesProducersIfQuantityEnough()
        {
            ClickerPlayer player = engine.GlobalProperties["player"] as ClickerPlayer;
            player.Points.Quantity = BigDouble.One;

            Assert.AreEqual(BigDouble.One, engine.GetProperty<BigDouble>("player.Points.Quantity"));
            engine.EvaluateCommand("buyProducer 1 1");
            Assert.AreEqual(BigDouble.Zero, engine.GetProperty<BigDouble>("player.Points.Quantity"));
        }

        [Test]
        public void buyProducerCommandThrowsIfQuantityNotEnough()
        {
            Assert.AreEqual(BigDouble.Zero, engine.GetProperty<BigDouble>("player.Points.Quantity"));
            var thrown = Assert.Throws(typeof(InvalidOperationException), () =>
            {
                engine.EvaluateCommand("buyProducer 1 1");
            });
            Assert.AreEqual("Need 1 points but had 0. (Usage: buyProducer [producerId] [quantity])", thrown.Message);
        }

        [Test]
        public void LoseProducerCommandMustHaveProducerName()
        {
            var thrown = Assert.Throws(typeof(InvalidOperationException), () =>
            {
                engine.EvaluateCommand("loseProducer");
            });
            Assert.AreEqual("Need producer id in position 1. (Usage: loseProducer [producerId] [quantity])", thrown.Message);
        }

        [Test]
        public void LoseProducerCommandMustHaveQuantity()
        {
            var thrown = Assert.Throws(typeof(InvalidOperationException), () =>
            {
                engine.EvaluateCommand("loseProducer Producer");
            });
            Assert.AreEqual("Need producer quantity in position 2. (Usage: loseProducer [producerId] [quantity])", thrown.Message);
        }

        [Test]
        public void LoseProducerCommandReducesProducers()
        {

            ClickerPlayer player = engine.GlobalProperties["player"] as ClickerPlayer;
            player.Producers[1].Quantity = 1;

            Assert.AreEqual(BigDouble.One, engine.GetProperty<BigDouble>("player.Producers[1].Quantity"));
            engine.EvaluateCommand("loseProducer 1 1");
            Assert.AreEqual(BigDouble.Zero, engine.GetProperty<BigDouble>("player.Producers[1].Quantity"));
        }

        [Test]
        public void LoseProducerCommandReducesProducersToNegative()
        {

            ClickerPlayer player = engine.GlobalProperties["player"] as ClickerPlayer;

            Assert.AreEqual(BigDouble.Zero, engine.GetProperty<BigDouble>("player.Producers[1].Quantity"));
            engine.EvaluateCommand("loseProducer 1 1");
            Assert.AreEqual(new BigDouble(-1), engine.GetProperty<BigDouble>("player.Producers[1].Quantity"));
        }
    }
}