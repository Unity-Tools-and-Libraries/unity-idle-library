using BreakInfinity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework
{
    public class Sum : ValueContainer
    {
        private readonly ValueContainer[] operands;

        private Sum(params ValueContainer[] operands)
        {
            this.operands = operands;
        }
        public static Sum Of(params ValueContainer[] operands)
        {
            return new Sum(operands);
        }

        public bool GetAsBoolean(IdleEngine toCheck)
        {
            bool value = false;
            foreach(var operand in operands)
            {
                value = value && operand.GetAsBoolean(toCheck);
            }
            return value;
        }

        public PropertyContainer GetAsContainer(IdleEngine engine)
        {
            return null;
        }

        public BigDouble GetAsNumber(IdleEngine engine)
        {
            BigDouble value = 0;
            foreach(var operand in operands)
            {
                value += operand.GetAsNumber(engine);
            }
            return value;
        }

        public string GetAsString(IdleEngine engine)
        {
            return GetAsNumber(engine).ToString();
        }

        public object RawValue(IdleEngine engine)
        {
            return GetAsNumber(engine);
        }
    }
}