using BreakInfinity;

namespace IdleFramework
{
    public class EntityNumberPropertyMatcher : NumberPropertyMatcher
    {
        public EntityNumberPropertyMatcher(string entityKey, string entityProperty, Comparison comparisonOperator, NumberContainer valueToCompareAgainst) : base(new EntityNumberPropertyReference(entityKey, entityProperty), comparisonOperator, valueToCompareAgainst)
        {
            
        }

        public EntityNumberPropertyMatcher(string entityKey, string entityProperty, Comparison comparisonOperator, BigDouble valueToCompareAgainst) : base(new EntityNumberPropertyReference(entityKey, entityProperty), comparisonOperator, Literal.Of(valueToCompareAgainst))
        {

        }
    }
}