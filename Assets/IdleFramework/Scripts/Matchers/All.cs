using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework
{
    public class All : StateMatcher
    {
        private StateMatcher[] matchers;

        private All(params StateMatcher[] matchers)
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

        public static All Of(params StateMatcher[] matchers)
        {
            return new All(matchers);
        }
    }
}