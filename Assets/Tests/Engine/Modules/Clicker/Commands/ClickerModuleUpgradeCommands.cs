using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace io.github.thisisnozaku.idle.framework.Tests.Engine.Modules.Clicker
{
    public class ClickerModuleUpgradeCommands : ClickerModuleTestsBase
    {
        [Test]
        public void GainUpgradeCommandMustHaveUpgradeName()
        {
            Configure();
            var thrown = Assert.Throws(typeof(InvalidOperationException), () =>
            {
                engine.EvaluateCommand("gainUpgrade");
            });
            Assert.AreEqual("Need upgrade id in position 1. Usage: gainUpgrade [upgradeId] [quantity]", thrown.Message);
        }

        [Test]
        public void GainUpgradeCommandMustHaveQuantity()
        {
            Configure();
            var thrown = Assert.Throws(typeof(InvalidOperationException), () =>
            {
                engine.EvaluateCommand("gainUpgrade Upgrade");
            });
            Assert.AreEqual("Need upgrade quantity in position 2. Usage: gainUpgrade [upgradeId] [quantity]", thrown.Message);
        }

        [Test]
        public void GainUpgradeCommandIncreasesUpgrade()
        {
            Configure();

            Assert.AreEqual(BigDouble.Zero, engine.GetProperty<BigDouble>("player.Upgrades[2].Quantity"));
            engine.EvaluateCommand("gainUpgrade 2 1");
            Assert.AreEqual(BigDouble.One, engine.GetProperty<BigDouble>("player.Upgrades[2].Quantity"));
        }

        [Test]
        public void buyUpgradeCommandMustHaveUpgradeName()
        {
            Configure();
            var thrown = Assert.Throws(typeof(InvalidOperationException), () =>
            {
                engine.EvaluateCommand("buyUpgrade");
            });
            Assert.AreEqual("Need upgrade id in position 1. Usage: buyUpgrade [upgradeId] [quantity]", thrown.Message);
        }

        [Test]
        public void buyUpgradeCommandMustHaveQuantity()
        {
            Configure();
            var thrown = Assert.Throws(typeof(InvalidOperationException), () =>
            {
                engine.EvaluateCommand("buyUpgrade Upgrade");
            });
            Assert.AreEqual("Need upgrade quantity in position 2. Usage: buyUpgrade [upgradeId] [quantity]", thrown.Message);
        }

        [Test]
        public void buyUpgradeCommandReducesUpgradesIfQuantityEnough()
        {
            Configure();

            ClickerPlayer player = engine.GlobalProperties["player"] as ClickerPlayer;
            player.Points.Quantity = BigDouble.One;

            Assert.AreEqual(BigDouble.One, engine.GetProperty<BigDouble>("player.Points.Quantity"));
            engine.EvaluateCommand("buyUpgrade 2 1");
            Assert.AreEqual(BigDouble.Zero, engine.GetProperty<BigDouble>("player.Points.Quantity"));
        }

        [Test]
        public void buyUpgradeCommandThrowsIfQuantityNotEnough()
        {
            Configure();

            Assert.AreEqual(BigDouble.Zero, engine.GetProperty<BigDouble>("player.Points.Quantity"));
            var thrown = Assert.Throws(typeof(InvalidOperationException), () =>
            {
                engine.EvaluateCommand("buyUpgrade 2 1");
            });
            Assert.AreEqual("Need 1 points but had 0. Usage: buyUpgrade [upgradeId] [quantity]", thrown.Message);
        }

        [Test]
        public void LoseUpgradeCommandMustHaveUpgradeName()
        {
            Configure();
            var thrown = Assert.Throws(typeof(InvalidOperationException), () =>
            {
                engine.EvaluateCommand("loseUpgrade");
            });
            Assert.AreEqual("Need upgrade id in position 1. Usage: loseUpgrade [upgradeId] [quantity]", thrown.Message);
        }

        [Test]
        public void LoseUpgradeCommandMustHaveQuantity()
        {
            Configure();
            var thrown = Assert.Throws(typeof(InvalidOperationException), () =>
            {
                engine.EvaluateCommand("loseUpgrade Upgrade");
            });
            Assert.AreEqual("Need upgrade quantity in position 2. Usage: loseUpgrade [upgradeId] [quantity]", thrown.Message);
        }

        [Test]
        public void LoseUpgradeCommandReducesUpgrades()
        {
            Configure();

            ClickerPlayer player = engine.GlobalProperties["player"] as ClickerPlayer;
            player.Upgrades[2].Quantity = 1;

            Assert.AreEqual(BigDouble.One, engine.GetProperty<BigDouble>("player.Upgrades[2].Quantity"));
            engine.EvaluateCommand("loseUpgrade 2 1");
            Assert.AreEqual(BigDouble.Zero, engine.GetProperty<BigDouble>("player.Upgrades[2].Quantity"));
        }

        [Test]
        public void LoseUpgradeCommandCannotReduceUpgradesToNegative()
        {
            Configure();

            ClickerPlayer player = engine.GlobalProperties["player"] as ClickerPlayer;

            Assert.AreEqual(BigDouble.Zero, engine.GetProperty<BigDouble>("player.Upgrades[2].Quantity"));
            engine.EvaluateCommand("loseUpgrade 2 1");
            Assert.AreEqual(BigDouble.Zero, engine.GetProperty<BigDouble>("player.Upgrades[2].Quantity"));
        }
    }

}