using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework
{
    public class GlobalNumberPropertyMatcher : NumberPropertyMatcher
    {
        public GlobalNumberPropertyMatcher(string propertyName, Comparison comparisonOperator, ValueContainer valueToCompareAgainst) : base(new GlobalPropertyReference(propertyName), comparisonOperator, valueToCompareAgainst)
        {
        }
    }
}