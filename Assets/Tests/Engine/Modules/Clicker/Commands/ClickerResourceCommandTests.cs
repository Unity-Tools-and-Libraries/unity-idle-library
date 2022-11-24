using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace io.github.thisisnozaku.idle.framework.Tests.Engine.Modules.Clicker
{
    public class ClickerResourceCommandTests : ClickerModuleTestsBase
    {
        [Test]
        public void GainResourceCommandMustHaveResourceName()
        {
            Configure();
            var thrown = Assert.Throws(typeof(InvalidOperationException), () =>
            {
                engine.EvaluateCommand("gainResource");
            });
            Assert.AreEqual("Need resource id in position 1. Usage: gainResource [resourceId] [quantity]", thrown.Message);
        }

        [Test]
        public void GainResourceCommandMustHaveQuantity()
        {
            Configure();
            var thrown = Assert.Throws(typeof(InvalidOperationException), () =>
            {
                engine.EvaluateCommand("gainResource resource");
            });
            Assert.AreEqual("Need resource quantity in position 2. Usage: gainResource [resourceId] [quantity]", thrown.Message);
        }

        [Test]
        public void GainResourceCommandIncreasesResource()
        {
            Configure();

            Assert.AreEqual(BigDouble.Zero, engine.GetProperty<BigDouble>("player.Points.Quantity"));
            engine.EvaluateCommand("gainResource points 1");
            Assert.AreEqual(BigDouble.One, engine.GetProperty<BigDouble>("player.Points.Quantity"));
        }

        [Test]
        public void SpendResourceCommandMustHaveResourceName()
        {
            Configure();
            var thrown = Assert.Throws(typeof(InvalidOperationException), () =>
            {
                engine.EvaluateCommand("spendResource");
            });
            Assert.AreEqual("Need resource id in position 1. Usage: spendResource [resourceId] [quantity]", thrown.Message);
        }

        [Test]
        public void SpendResourceCommandMustHaveQuantity()
        {
            Configure();
            var thrown = Assert.Throws(typeof(InvalidOperationException), () =>
            {
                engine.EvaluateCommand("spendResource resource");
            });
            Assert.AreEqual("Need resource quantity in position 2. Usage: spendResource [resourceId] [quantity]", thrown.Message);
        }

        [Test]
        public void SpendResourceCommandReducesResourcesIfQuantityEnough()
        {
            Configure();

            ClickerPlayer player = engine.GlobalProperties["player"] as ClickerPlayer;
            player.Points.Quantity = BigDouble.One;

            Assert.AreEqual(BigDouble.One, engine.GetProperty<BigDouble>("player.Points.Quantity"));
            engine.EvaluateCommand("spendResource points 1");
            Assert.AreEqual(BigDouble.Zero, engine.GetProperty<BigDouble>("player.Points.Quantity"));
        }

        [Test]
        public void SpendResourceCommandThrowsIfQuantityNotEnough()
        {
            Configure();

            Assert.AreEqual(BigDouble.Zero, engine.GetProperty<BigDouble>("player.Points.Quantity"));
            var thrown = Assert.Throws(typeof(InvalidOperationException), () =>
            {
                engine.EvaluateCommand("spendResource points 1");
            });
            Assert.AreEqual("Need 1 points but had 0. Usage: spendResource [resourceId] [quantity]", thrown.Message);
        }

        [Test]
        public void LoseResourceCommandMustHaveResourceName()
        {
            Configure();
            var thrown = Assert.Throws(typeof(InvalidOperationException), () =>
            {
                engine.EvaluateCommand("loseResource");
            });
            Assert.AreEqual("Need resource id in position 1. Usage: loseResource [resourceId] [quantity]", thrown.Message);
        }

        [Test]
        public void LoseResourceCommandMustHaveQuantity()
        {
            Configure();
            var thrown = Assert.Throws(typeof(InvalidOperationException), () =>
            {
                engine.EvaluateCommand("loseResource resource");
            });
            Assert.AreEqual("Need resource quantity in position 2. Usage: loseResource [resourceId] [quantity]", thrown.Message);
        }

        [Test]
        public void LoseResourceCommandReducesResources()
        {
            Configure();

            ClickerPlayer player = engine.GlobalProperties["player"] as ClickerPlayer;
            player.Points.Quantity = BigDouble.One;

            Assert.AreEqual(BigDouble.One, engine.GetProperty<BigDouble>("player.Points.Quantity"));
            engine.EvaluateCommand("loseResource points 1");
            Assert.AreEqual(BigDouble.Zero, engine.GetProperty<BigDouble>("player.Points.Quantity"));
        }

        [Test]
        public void LoseResourceCommandReducesResourcesToNegative()
        {
            Configure();

            ClickerPlayer player = engine.GlobalProperties["player"] as ClickerPlayer;

            Assert.AreEqual(BigDouble.Zero, engine.GetProperty<BigDouble>("player.Points.Quantity"));
            engine.EvaluateCommand("loseResource points 1");
            Assert.AreEqual(new BigDouble(-1), engine.GetProperty<BigDouble>("player.Points.Quantity"));
        }
    }
}