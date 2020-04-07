using BreakInfinity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework
{
    public class CalculatedValue : ValueContainer
    {
        private readonly Func<IdleEngine, string> stringCalculator;
        private readonly Func<IdleEngine, BigDouble> numberCalculator;
        private readonly Func<IdleEngine, bool> booleanCalculator;

        public CalculatedValue(Func<IdleEngine, string> stringCalculator, Func<IdleEngine, BigDouble> numberCalculator, Func<IdleEngine, bool> booleanCalculator)
        {
            this.stringCalculator = stringCalculator;
            this.numberCalculator = numberCalculator;
            this.booleanCalculator = booleanCalculator;
        }

        public bool GetAsBoolean(IdleEngine engine)
        {
            return booleanCalculator(engine);
        }

        public PropertyContainer GetAsContainer(IdleEngine engine)
        {
            return null;
        }

        public BigDouble GetAsNumber(IdleEngine engine)
        {
            return numberCalculator(engine);
        }

        public string GetAsString(IdleEngine engine)
        {
            return stringCalculator(engine);
        }

        public object RawValue(IdleEngine engine)
        {
            throw new NotImplementedException();
        }
    }
}