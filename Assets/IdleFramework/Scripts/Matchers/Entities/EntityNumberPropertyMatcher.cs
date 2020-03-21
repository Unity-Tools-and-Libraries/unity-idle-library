using BreakInfinity;

namespace IdleFramework
{
    public class EntityNumberPropertyMatcher : NumberPropertyMatcher
    {
        

        public EntityNumberPropertyMatcher(string entityKey, string entityProperty, string entitySubProperty, Comparison comparisonOperator, ValueContainer valueToCompareAgainst) : base(new EntityPropertyReference(entityKey, entityProperty, entitySubProperty), comparisonOperator,  valueToCompareAgainst)
        {
            
        }

        public EntityNumberPropertyMatcher(string entityKey, string entityProperty, string entitySubProperty, Comparison comparisonOperator, BigDouble valueToCompareAgainst) : this(entityKey, entityProperty, entitySubProperty, comparisonOperator, Literal.Of(valueToCompareAgainst))
        {
        }

        public EntityNumberPropertyMatcher(string entityKey, string entityProperty, string entitySubProperty, Comparison comparisonOperator, int valueToCompareAgainst) : this(entityKey, entityProperty, entitySubProperty, comparisonOperator, Literal.Of(valueToCompareAgainst))
        {
            
        }

        public EntityNumberPropertyMatcher(string entityKey, string entityProperty, Comparison comparisonOperator, ValueContainer valueToCompareAgainst) : this(entityKey, entityProperty, "", comparisonOperator, valueToCompareAgainst)
        {
            
        }

        public EntityNumberPropertyMatcher(string entityKey, string entityProperty, Comparison comparisonOperator, BigDouble valueToCompareAgainst) : this(entityKey, entityProperty, "", comparisonOperator, valueToCompareAgainst)
        {
            
        }

        public EntityNumberPropertyMatcher(string entityKey, string entityProperty, Comparison comparisonOperator, int valueToCompareAgainst) : this(entityKey, entityProperty, "", comparisonOperator, valueToCompareAgainst)
        {
            
        }
    }
}