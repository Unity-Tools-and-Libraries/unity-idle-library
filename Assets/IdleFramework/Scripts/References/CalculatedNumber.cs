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
    public class CalculatedNumber : CalculatedValue
    {
        private Func<IdleEngine, BigDouble> calcuatingFunction;

        public CalculatedNumber(Func<IdleEngine, BigDouble> calcuatingFunction): base(engine => {
            BigDouble value = calcuatingFunction(engine);
            return value.ToString();
        }, calcuatingFunction, engine => {
            return !BigDouble.Zero.Equals(calcuatingFunction(engine));
        })
        {
            this.calcuatingFunction = calcuatingFunction;
        }
    }
}