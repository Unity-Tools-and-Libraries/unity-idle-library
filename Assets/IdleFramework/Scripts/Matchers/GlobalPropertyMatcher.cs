using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework
{
    public abstract class GlobalPropertyMatcher : StateMatcher
    {
        private readonly string propertyName;

        protected GlobalPropertyMatcher(string propertyName)
        {
            this.propertyName = propertyName;
        }

        public string PropertyName => propertyName;

        public abstract bool Matches(IdleEngine toCheck);
    }
}