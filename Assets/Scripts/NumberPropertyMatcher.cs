using System;
namespace IdleFramework
{
    public abstract class NumberPropertyMatcher : StateMatcher
    {
        private readonly NumberContainer toCompare;
        private readonly NumberContainer toCompareAgainst;
        private readonly Comparison comparisonOperator;

        protected NumberPropertyMatcher(NumberContainer toCompare, Comparison comparisonOperator, NumberContainer toCompareAgainst)
        {
            this.toCompare = toCompare;
            this.toCompareAgainst = toCompareAgainst;
            this.comparisonOperator = comparisonOperator;
        }

        public bool Matches(IdleEngine toCheck)
        {
            var left = toCompare.Get(toCheck);
            var right = toCompareAgainst.Get(toCheck);
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