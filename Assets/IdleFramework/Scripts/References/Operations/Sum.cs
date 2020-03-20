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

        public BigDouble GetAsNumber(IdleEngine engine)
        {
            BigDouble value = 0;
            foreach(var operand in operands)
            {
                value += operand.GetAsNumber(engine);
            }
            return value;
        }
    }
}