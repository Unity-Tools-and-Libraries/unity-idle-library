using BreakInfinity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework
{
    public class Sum : NumberContainer
    {
        private readonly NumberContainer[] operands;

        private Sum(params NumberContainer[] operands)
        {
            this.operands = operands;
        }
        public static Sum Of(params NumberContainer[] operands)
        {
            return new Sum(operands);
        }

        public BigDouble Get(IdleEngine engine)
        {
            BigDouble value = 0;
            foreach(var operand in operands)
            {
                value += operand.Get(engine);
            }
            return value;
        }
    }
}