using BreakInfinity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework
{
    public class Difference : NumberContainer
    {
        private NumberContainer left;
        private NumberContainer right;

        public Difference(NumberContainer left, NumberContainer right)
        {
            this.left = left;
            this.right = right;
        }

        public BigDouble Get(IdleEngine engine)
        {
            var leftHand = left.Get(engine);
            var rightHand = right.Get(engine);
            return leftHand - rightHand;
        }

        public static Difference Between(NumberContainer left, NumberContainer right)
        {
            return new Difference(left, right);
        }
    }
}