
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
        bool Matches(IdleEngine toCheck);
    }

    public enum Comparison
    {
        EQUALS,
        GREATER_THAN,
        LESS_THAN
    }

}