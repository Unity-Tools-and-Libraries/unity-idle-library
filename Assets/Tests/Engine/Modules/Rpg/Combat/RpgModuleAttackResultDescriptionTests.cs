using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg.Combat;
using NUnit.Framework;
using System.Linq;

namespace io.github.thisisnozaku.idle.framework.Tests.Engine.Modules.Rpg
{
    public class RpgModuleAttackResultDescriptionTests
    {
        [Test]
        public void CanClearDamageToTheAttacker()
        {
            var dmg = new AttackResultDescription(true, "", 1, null, null, null);
            dmg.DamageAttacker(BigDouble.One, null);
            dmg.ClearAttackerDamage();

            Assert.AreEqual(0, dmg.DamageToAttacker.Count);
        }

        [Test]
        public void CanAddDamageToAttacker()
        {
            var dmg = new AttackResultDescription(true, "", 1, null, null, null);
            dmg.DamageAttacker(BigDouble.One, null);

            Assert.AreEqual(1, dmg.DamageToAttacker.Count);
            Assert.AreEqual(BigDouble.One, dmg.DamageToAttacker.Select(x => x.Item1).Aggregate((x, y) => x.Add(y)));
        }

        [Test]
        public void CanAddDamageToDefender()
        {
            var dmg = new AttackResultDescription(true, "", 1, null, null, null);
            dmg.DamageDefender(BigDouble.One, null);

            Assert.AreEqual(2, dmg.DamageToDefender.Count);
            Assert.AreEqual(new BigDouble(2), dmg.DamageToDefender.Select(x => x.Item1).Aggregate((x, y) => x.Add(y)));
        }

        [Test]
        public void DescribeWithNoDamage()
        {
            var dmg = new AttackResultDescription(true, "", 1, null, null, null);
            dmg.ClearDefenderDamage();
            Assert.AreEqual("hit defender for 0 damage", dmg.ToString());
        }

        [Test]
        public void DescribeHit()
        {
            var dmg = new AttackResultDescription(true, "", 1, null, null, null);
            Assert.AreEqual("hit defender for 1 damage", dmg.ToString());
        }

        [Test]
        public void DescribeHitWithStatus()
        {
            var dmg = new AttackResultDescription(true, "", 1, null, null, new System.Collections.Generic.List<long>()
            {
                1
            });
            Assert.AreEqual("hit defender for 1 damage, applied status(es) 1 to defender", dmg.ToString());
        }

        [Test]
        public void DescribeHitWithDamageToBoth()
        {
            var dmg = new AttackResultDescription(true, "", 1, null, null, null);
            dmg.DamageAttacker(1, null);
            Assert.AreEqual("hit defender for 1 damage and attacker for 1 damage", dmg.ToString());
        }
    }
}