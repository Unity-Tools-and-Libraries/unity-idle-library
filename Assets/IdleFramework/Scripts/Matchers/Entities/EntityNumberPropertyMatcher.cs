using BreakInfinity;

namespace IdleFramework
{
    public class EntityNumberPropertyMatcher : NumberPropertyMatcher
    {
        public EntityNumberPropertyMatcher(string entityKey, string entityProperty, Comparison comparisonOperator, ValueContainer valueToCompareAgainst) : base(new EntityPropertyReference(entityKey, entityProperty), comparisonOperator, valueToCompareAgainst)
        {
            
        }

        public EntityNumberPropertyMatcher(string entityKey, string entityProperty, Comparison comparisonOperator, BigDouble valueToCompareAgainst) : base(new EntityPropertyReference(entityKey, entityProperty), comparisonOperator, Literal.Of(valueToCompareAgainst))
        {

        }
    }
}