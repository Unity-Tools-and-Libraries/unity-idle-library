using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace IdleFramework
{
    public class Negate : StateMatcher
    {
        private readonly StateMatcher matcher;

        public Negate(StateMatcher matcher)
        {
            this.matcher = matcher;
        }

        public bool Matches(IdleEngine engine)
        {
            return !matcher.Matches(engine);
        }
    }
}