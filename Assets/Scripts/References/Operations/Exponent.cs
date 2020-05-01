using BreakInfinity;
using IdleFramework;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace IdleFramework
{
    public class Exponent : NumberContainer
    {
        private readonly NumberContainer @base;
        private readonly NumberContainer power;
        private Exponent(NumberContainer @base, NumberContainer power)
        {
            this.@base = @base;
            this.power = power;
        }

        public BigDouble Get(IdleEngine engine)
        {
            BigDouble baseNumber = @base.Get(engine);
            BigDouble exponent = power.Get(engine);
            return BigDouble.Pow(baseNumber, exponent);
        }

        public static Exponent Of(NumberContainer @base, NumberContainer power)
        {
            return new Exponent(@base, power);
        }
    }
}