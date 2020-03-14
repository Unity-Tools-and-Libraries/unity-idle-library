using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework
{
    /*
     * A matcher which wraps other matchers, returning true if one or more of the wrapped matchers is a match.
     */
    public class AnyMatcher : StateMatcher
    {
        private StateMatcher[] matchers;
        public AnyMatcher(params StateMatcher[] matchers)
        {
            this.matchers = matchers;
        }
        public bool Matches(IdleEngine toCheck)
        {
            foreach(var matcher in matchers)
            {
                if (matcher.Matches(toCheck)){
                    return true;
                }
            }
            return false;
        }

        public static AnyMatcher AnyOf(params StateMatcher[] matchers)
        {
            return new AnyMatcher(matchers);
        }
    }
}