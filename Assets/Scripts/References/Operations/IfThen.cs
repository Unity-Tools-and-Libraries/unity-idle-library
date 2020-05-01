using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework
{
    public class IfThen : ValueContainer
    {
        private readonly StateMatcher matcher;
        private readonly ValueContainer left;
        private readonly ValueContainer right;

        public IfThen(StateMatcher matcher, ValueContainer left, ValueContainer right)
        {
            this.matcher = matcher;
            this.left = left;
            this.right = right;
        }
    }
}