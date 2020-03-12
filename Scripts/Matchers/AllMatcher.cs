using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework
{
    public class AllMatcher : StateMatcher
    {
        private StateMatcher[] matchers;

        public AllMatcher(StateMatcher[] matchers)
        {
            this.matchers = matchers;
        }

        public bool Matches(IdleEngine toCheck)
        {
            foreach(var matcher in matchers)
            {
                if(!matcher.Matches(toCheck))
                {
                    return false;
                }
            }
            return true;
        }

        public static AllMatcher AllOf(params StateMatcher[] matchers)
        {
            return new AllMatcher(matchers);
        }
    }
}