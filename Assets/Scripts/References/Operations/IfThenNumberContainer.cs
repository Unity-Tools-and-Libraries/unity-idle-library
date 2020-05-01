using BreakInfinity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework
{
    public class IfThenNumberContainer : NumberContainer
    {
        private readonly StateMatcher condition;
        private readonly NumberContainer ifTrue;
        private readonly NumberContainer ifFalse;

        public IfThenNumberContainer(StateMatcher condition, NumberContainer ifTrue, NumberContainer ifFalse)
        {
            this.condition = condition;
            this.ifTrue = ifTrue;
            this.ifFalse = ifFalse;
        }

        public BigDouble Get(IdleEngine engine)
        {
            return condition.Matches(engine) ? ifTrue.Get(engine) : ifFalse.Get(engine);
        }
    }
}