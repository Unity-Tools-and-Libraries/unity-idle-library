
namespace IdleFramework
{
    /*
     * An object which attempts to determine if a checked instance has an expected state.
     */
    public interface StateMatcher
    {
        /*
         * Returns true if the given object matches, false otherwise.
         */
        bool Matches(IdleEngine engine);
    }

    public enum Comparison
    {
        EQUALS,
        GREATER_THAN,
        GREATER_THAN_OR_EQUAL,
        LESS_THAN,
        LESS_THAN_OR_EQUAL
    }

    public static class StateMatcherExtensions
    {
        public static StateMatcher And(this StateMatcher left, StateMatcher right)
        {
            return All.Of(left, right);
        }

        public static StateMatcher Or(this StateMatcher left, StateMatcher right)
        {
            return Any.Of(left, right);
        }
    }

}