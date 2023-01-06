using BreakInfinity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            if (attackDamage > 0)
            {
                OriginalDamageToDefender = Tuple.Create(attackDamage, attacker);
                DamageToDefender = new List<Tuple<BigDouble, RpgCharacter>>()
                {
                    OriginalDamageToDefender
                };
            } else
            {
                DamageToDefender = new List<Tuple<BigDouble, RpgCharacter>>();
            }
            DamageToAttacker = new List<Tuple<BigDouble, RpgCharacter>>();
            
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

        public override string ToString()
        {
            string hitOrMiss = this.IsHit ? "hit" : "miss";
            BigDouble totalDamageToTarget = DamageToDefender.Count > 0 ? DamageToDefender.Select(x => x.Item1).Aggregate((a, b) => a.Add(b)) : 0;
            string statuses = StatusesToApplyToDefender != null &&
                StatusesToApplyToDefender.Count > 0 ?
                string.Join(", ", StatusesToApplyToDefender) :
                "";
            if(statuses.Length > 0)
            {
                return string.Format("{0} defender for {1} damage, applied status(es) {2} to defender", hitOrMiss, totalDamageToTarget, statuses);
            }
            return string.Format("{0} defender for {1} damage", hitOrMiss, totalDamageToTarget);
        }
    }
}