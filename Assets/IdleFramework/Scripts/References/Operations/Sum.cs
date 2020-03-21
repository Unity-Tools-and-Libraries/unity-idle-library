using BreakInfinity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework
{
    public class Sum : PropertyReference
    {
        private readonly PropertyReference[] operands;

        private Sum(params PropertyReference[] operands)
        {
            this.operands = operands;
        }
        public static Sum Of(params PropertyReference[] operands)
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