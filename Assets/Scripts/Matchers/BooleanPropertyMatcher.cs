namespace IdleFramework
{
    public class BooleanPropertyMatcher : StateMatcher
    {
        private readonly BooleanContainer toCompare;
        private readonly BooleanContainer toCompareAgainst;

        public BooleanPropertyMatcher(BooleanContainer toCompare, BooleanContainer toCompareAgainst)
        {
            this.toCompare = toCompare;
            this.toCompareAgainst = toCompareAgainst;
        }

        public ValueContainer ToCompareAgainst => toCompareAgainst;

        public ValueContainer ToCompare => toCompare;

        public bool Matches(IdleEngine toCheck)
        {
            bool left = toCompare.Get(toCheck);
            bool right = toCompareAgainst.Get(toCheck);
            return left == true;
        }
    }
}