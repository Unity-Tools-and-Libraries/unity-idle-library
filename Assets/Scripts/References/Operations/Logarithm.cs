using BreakInfinity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework
{
    public class Logarithm : NumberContainer
    {
        private readonly NumberContainer logarithmBase;
        private readonly NumberContainer input;

        private Logarithm(NumberContainer logarithmBase, NumberContainer input)
        {
            this.logarithmBase = logarithmBase;
            this.input = input;
        }
        public BigDouble Get(IdleEngine engine)
        {
            return BigDouble.Log(input.Get(engine), logarithmBase.Get(engine));
        }

        public static NumberContainer Of(NumberContainer @base, NumberContainer value )
        {
            return new Logarithm(@base, value);
        }
    }
}