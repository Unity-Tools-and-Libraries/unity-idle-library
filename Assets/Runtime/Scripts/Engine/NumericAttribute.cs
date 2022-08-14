using System.Collections;
using System.Collections.Generic;
using BreakInfinity;
using UnityEngine;
namespace io.github.thisisnozaku.idle.framework.Engine
{
    /*
     * This is a convenience class for a common scenario in many idle games: a numeric property which has a base value,
     * a level which is multiplied by some per-level delta and added on top of the base value; this value is then finally
     * multiplied by some value.
     */
    public class NumericAttribute
    {
        public BigDouble BaseValue = 0;
        public BigDouble Multiplier = 1;
        public BigDouble ChangePerLevel = 1;
        public BigDouble Level = 1;

        public NumericAttribute(BigDouble perLevelValue, BigDouble? multiplier = null, BigDouble? baseValue = null, BigDouble? level = null)
        {
            this.ChangePerLevel = perLevelValue;
            this.BaseValue = baseValue.HasValue ? baseValue.Value : this.BaseValue;
            this.Multiplier = multiplier.HasValue ? multiplier.Value : 1;
            this.Level = level.HasValue ? level.Value : 1;
        }

        public BigDouble Total => (BaseValue + (ChangePerLevel * (Level - 1 ))) * Multiplier;

        public static implicit operator BigDouble(NumericAttribute attr) => attr.Total;
    }
}