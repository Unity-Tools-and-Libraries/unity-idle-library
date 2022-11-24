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
        [Test]
        public void GainProducerCommandMustHaveProducerName()
        {
            Configure();
            var thrown = Assert.Throws(typeof(InvalidOperationException), () =>
            {
                engine.EvaluateCommand("gainProducer");
            });
            Assert.AreEqual("Need producer id in position 1. Usage: gainProducer [producerId] [quantity]", thrown.Message);
        }

        [Test]
        public void GainProducerCommandMustHaveQuantity()
        {
            Configure();
            var thrown = Assert.Throws(typeof(InvalidOperationException), () =>
            {
                engine.EvaluateCommand("gainProducer Producer");
            });
            Assert.AreEqual("Need producer quantity in position 2. Usage: gainProducer [producerId] [quantity]", thrown.Message);
        }

        [Test]
        public void GainProducerCommandIncreasesProducer()
        {
            Configure();

            Assert.AreEqual(BigDouble.Zero, engine.GetProperty<BigDouble>("player.Producers[1].Quantity"));
            engine.EvaluateCommand("gainProducer 1 1");
            Assert.AreEqual(BigDouble.One, engine.GetProperty<BigDouble>("player.Producers[1].Quantity"));
        }

        [Test]
        public void buyProducerCommandMustHaveProducerName()
        {
            Configure();
            var thrown = Assert.Throws(typeof(InvalidOperationException), () =>
            {
                engine.EvaluateCommand("buyProducer");
            });
            Assert.AreEqual("Need producer id in position 1. Usage: buyProducer [producerId] [quantity]", thrown.Message);
        }

        [Test]
        public void buyProducerCommandMustHaveQuantity()
        {
            Configure();
            var thrown = Assert.Throws(typeof(InvalidOperationException), () =>
            {
                engine.EvaluateCommand("buyProducer Producer");
            });
            Assert.AreEqual("Need producer quantity in position 2. Usage: buyProducer [producerId] [quantity]", thrown.Message);
        }

        [Test]
        public void buyProducerCommandReducesProducersIfQuantityEnough()
        {
            Configure();

            ClickerPlayer player = engine.GlobalProperties["player"] as ClickerPlayer;
            player.Points.Quantity = BigDouble.One;

            Assert.AreEqual(BigDouble.One, engine.GetProperty<BigDouble>("player.Points.Quantity"));
            engine.EvaluateCommand("buyProducer 1 1");
            Assert.AreEqual(BigDouble.Zero, engine.GetProperty<BigDouble>("player.Points.Quantity"));
        }

        [Test]
        public void buyProducerCommandThrowsIfQuantityNotEnough()
        {
            Configure();

            Assert.AreEqual(BigDouble.Zero, engine.GetProperty<BigDouble>("player.Points.Quantity"));
            var thrown = Assert.Throws(typeof(InvalidOperationException), () =>
            {
                engine.EvaluateCommand("buyProducer 1 1");
            });
            Assert.AreEqual("Need 1 points but had 0. Usage: buyProducer [producerId] [quantity]", thrown.Message);
        }

        [Test]
        public void LoseProducerCommandMustHaveProducerName()
        {
            Configure();
            var thrown = Assert.Throws(typeof(InvalidOperationException), () =>
            {
                engine.EvaluateCommand("loseProducer");
            });
            Assert.AreEqual("Need producer id in position 1. Usage: loseProducer [producerId] [quantity]", thrown.Message);
        }

        [Test]
        public void LoseProducerCommandMustHaveQuantity()
        {
            Configure();
            var thrown = Assert.Throws(typeof(InvalidOperationException), () =>
            {
                engine.EvaluateCommand("loseProducer Producer");
            });
            Assert.AreEqual("Need producer quantity in position 2. Usage: loseProducer [producerId] [quantity]", thrown.Message);
        }

        [Test]
        public void LoseProducerCommandReducesProducers()
        {
            Configure();

            ClickerPlayer player = engine.GlobalProperties["player"] as ClickerPlayer;
            player.Producers[1].Quantity = 1;

            Assert.AreEqual(BigDouble.One, engine.GetProperty<BigDouble>("player.Producers[1].Quantity"));
            engine.EvaluateCommand("loseProducer 1 1");
            Assert.AreEqual(BigDouble.Zero, engine.GetProperty<BigDouble>("player.Producers[1].Quantity"));
        }

        [Test]
        public void LoseProducerCommandReducesProducersToNegative()
        {
            Configure();

            ClickerPlayer player = engine.GlobalProperties["player"] as ClickerPlayer;

            Assert.AreEqual(BigDouble.Zero, engine.GetProperty<BigDouble>("player.Producers[1].Quantity"));
            engine.EvaluateCommand("loseProducer 1 1");
            Assert.AreEqual(new BigDouble(-1), engine.GetProperty<BigDouble>("player.Producers[1].Quantity"));
        }
    }
}