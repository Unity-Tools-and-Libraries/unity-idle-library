using BreakInfinity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework
{
    /*
     * 
     */
    public class CalculatedNumber : NumberContainer
    {
        private Func<IdleEngine, BigDouble> calcuatingFunction;

        public CalculatedNumber(Func<IdleEngine, BigDouble> calcuatingFunction)
        {
            this.calcuatingFunction = calcuatingFunction;
        }

        public BigDouble Get(IdleEngine engine)
        {
            return calcuatingFunction.Invoke(engine);
        }
    }
}