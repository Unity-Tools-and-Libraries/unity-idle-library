using BreakInfinity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg.Combat
{
    public class AttackResultDescription
    {
        public bool IsHit;
        public string Description;
        public BigDouble OriginalDamageToDefender;
        public BigDouble DamageToDefender;
        public BigDouble DamageToAttacker;
        public List<long> StatusesToApplyToAttacker;
        public List<long> StatusesToApplyToDefender;

        public AttackResultDescription(bool isHit, string description, BigDouble damageToDefender, BigDouble damageToAttacker, List<long> statusesToApplyToAttacker, List<long> statusesToApplyToDefender)
        {
            IsHit = isHit;
            Description = description;
            OriginalDamageToDefender = DamageToDefender = damageToDefender;
            DamageToAttacker = damageToAttacker;
            StatusesToApplyToAttacker = statusesToApplyToAttacker;
            StatusesToApplyToDefender = statusesToApplyToDefender;
        }
    }
}