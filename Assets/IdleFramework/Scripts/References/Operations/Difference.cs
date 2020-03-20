using BreakInfinity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework
{
    public class Difference : PropertyReference
    {
        private PropertyReference left;
        private PropertyReference right;

        public Difference(PropertyReference left, PropertyReference right)
        {
            this.left = left;
            this.right = right;
        }

        public BigDouble GetAsNumber(IdleEngine engine)
        {
            return left.GetAsNumber(engine) - right.GetAsNumber(engine);
        }

        public static Difference Of(PropertyReference left, PropertyReference right)
        {
            return new Difference(left, right);
        }
    }
}