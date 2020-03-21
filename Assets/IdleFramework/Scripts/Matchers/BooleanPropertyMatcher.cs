namespace IdleFramework
{
    public class BooleanPropertyMatcher : StateMatcher
    {
        private readonly ValueContainer toCompare;
        private readonly ValueContainer toCompareAgainst;

        public BooleanPropertyMatcher(ValueContainer toCompare, ValueContainer toCompareAgainst)
        {
            this.toCompare = toCompare;
            this.toCompareAgainst = toCompareAgainst;
        }

        public ValueContainer ToCompareAgainst => toCompareAgainst;

        public ValueContainer ToCompare => toCompare;

        public bool Matches(IdleEngine toCheck)
        {
            bool left = toCompare.GetAsBoolean(toCheck);
            bool right = toCompare.GetAsBoolean(toCheck);
            return left == true;
        }
    }
}