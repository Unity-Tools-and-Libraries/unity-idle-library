using System;
namespace IdleFramework
{
    public abstract class NumberPropertyMatcher : StateMatcher
    {
        private readonly ValueContainer toCompare;
        private readonly ValueContainer toCompareAgainst;
        private readonly Comparison comparisonOperator;

        protected NumberPropertyMatcher(ValueContainer toCompare, Comparison comparisonOperator, ValueContainer toCompareAgainst)
        {
            this.toCompare = toCompare;
            this.toCompareAgainst = toCompareAgainst;
            this.comparisonOperator = comparisonOperator;
        }

        public bool Matches(IdleEngine toCheck)
        {
            var left = toCompare.GetAsNumber(toCheck);
            var right = toCompareAgainst.GetAsNumber(toCheck);
            switch (comparisonOperator)
            {
                case Comparison.EQUALS:
                    return left.CompareTo(right) == 0;
                case Comparison.GREATER_THAN:
                    return left.CompareTo(right) > 0;
                case Comparison.GREATER_THAN_OR_EQUAL:
                    return left.CompareTo(right) >= 0;
                case Comparison.LESS_THAN:
                    return left.CompareTo(right) < 0;
                case Comparison.LESS_THAN_OR_EQUAL:
                    return left.CompareTo(right) <= 0;
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}