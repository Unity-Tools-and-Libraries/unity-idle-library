using BreakInfinity;
using IdleFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework.State.Matchers
{
    public class EntityQuantityMatcher : EntityNumberPropertyMatcher
    {
        public EntityQuantityMatcher(string entityKey, Comparison comparisonOperator, NumberContainer valueToCompareAgainst) : base(entityKey, "Quantity", comparisonOperator, valueToCompareAgainst)
        {
        }

        public EntityQuantityMatcher(string entityKey, Comparison comparisonOperator, BigDouble valueToCompareAgainst) : this(entityKey, comparisonOperator, Literal.Of(valueToCompareAgainst))
        {
        }
    }
}