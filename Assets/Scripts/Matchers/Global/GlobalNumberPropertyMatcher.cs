using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework
{
    public class GlobalNumberPropertyMatcher : NumberPropertyMatcher
    {
        public GlobalNumberPropertyMatcher(string propertyName, Comparison comparisonOperator, NumberContainer valueToCompareAgainst) : base(new GlobalNumberPropertyReference(propertyName), comparisonOperator, valueToCompareAgainst)
        {
        }
    }
}