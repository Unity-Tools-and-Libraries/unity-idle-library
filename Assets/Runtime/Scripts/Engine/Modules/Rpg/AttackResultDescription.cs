using BreakInfinity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg.Combat
{
    public class AttackResultDescription
    {
        public bool IsHit;
        public string Description;
        public Tuple<BigDouble, RpgCharacter> OriginalDamageToDefender;
        public List<Tuple<BigDouble, RpgCharacter>> DamageToDefender;
        public List<Tuple<BigDouble, RpgCharacter>> DamageToAttacker;
        public List<long> StatusesToApplyToAttacker;
        public List<long> StatusesToApplyToDefender;

        public AttackResultDescription(bool isHit, string description,
            BigDouble attackDamage, RpgCharacter attacker,
            List<long> statusesToApplyToAttacker, List<long> statusesToApplyToDefender)
        {
            IsHit = isHit;
            Description = description;
            OriginalDamageToDefender = Tuple.Create(attackDamage, attacker);
            DamageToAttacker = new List<Tuple<BigDouble, RpgCharacter>>();
            DamageToDefender = new List<Tuple<BigDouble, RpgCharacter>>()
            {
                OriginalDamageToDefender
            };
            
            StatusesToApplyToAttacker = statusesToApplyToAttacker;
            StatusesToApplyToDefender = statusesToApplyToDefender;
        }

        public void ClearAttackerDamage()
        {
            DamageToAttacker.Clear();
        }

        public void ClearDefenderDamage()
        {
            DamageToDefender.Clear();
        }

        public void DamageAttacker(BigDouble damage, RpgCharacter source)
        {
            DamageToAttacker.Add(Tuple.Create(damage, source));
        }

        public void DamageDefender(BigDouble damage, RpgCharacter source)
        {
            DamageToDefender.Add(Tuple.Create(damage, source));
        }
    }
}