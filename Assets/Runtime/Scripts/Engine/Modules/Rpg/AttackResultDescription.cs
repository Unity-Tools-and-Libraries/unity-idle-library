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
        private string toStringResultCache;

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
            toStringResultCache = null;
        }

        public void ClearDefenderDamage()
        {
            DamageToDefender.Clear();
            toStringResultCache = null;
        }

        public void DamageAttacker(BigDouble damage, RpgCharacter source)
        {
            DamageToAttacker.Add(Tuple.Create(damage, source));
            toStringResultCache = null;
        }

        public void DamageDefender(BigDouble damage, RpgCharacter source)
        {
            DamageToDefender.Add(Tuple.Create(damage, source));
            toStringResultCache = null;
        }

        public override string ToString()
        {
            if (toStringResultCache == null)
            {
                string hitOrMiss = this.IsHit ? "hit" : "miss";

                BigDouble totalDamageToTarget = DamageToDefender.Count > 0 ?
                    DamageToDefender.Select(x => x.Item1).Aggregate((a, b) => a.Add(b)) :
                    0;
                BigDouble? totalDamageToAttacker = DamageToAttacker.Count > 0 ?
                    DamageToAttacker.Select(x => x.Item1).Aggregate((a, b) => a.Add(b)) :
                    null;

                string damageMessage = string.Format("defender for {0} damage{1}", totalDamageToTarget,
                    totalDamageToAttacker.HasValue ? string.Format(" and attacker for {0} damage", totalDamageToAttacker.Value) :
                    "");

                string statuses = StatusesToApplyToDefender != null &&
                    StatusesToApplyToDefender.Count > 0 ?
                    string.Join(", ", StatusesToApplyToDefender) :
                    "";
                if (statuses.Length > 0)
                {
                    toStringResultCache = string.Format("{0} {1}, applied status(es) {2} to defender", hitOrMiss, damageMessage, statuses);
                }
                else
                {
                    toStringResultCache = string.Format("{0} {1}", hitOrMiss, damageMessage);
                }
            }

            return toStringResultCache;
        }
    }
}