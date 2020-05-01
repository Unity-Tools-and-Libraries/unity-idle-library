using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework
{
    public class DelegateStateMatcher : StateMatcher
    {
        private readonly Func<IdleEngine, bool> delegateFunc;

        public DelegateStateMatcher(Func<IdleEngine, bool> delegateFunc)
        {
            this.delegateFunc = delegateFunc;
        }

        public bool Matches(IdleEngine engine)
        {
            return delegateFunc.Invoke(engine);
        }
    }
}