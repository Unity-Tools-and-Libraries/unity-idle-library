using BreakInfinity;
using IdleFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework.Modifiers
{
    public abstract class Modifier
    {
        public readonly string Id;
        public readonly string Description;
        private object value;
        public Modifier(string id, string description, BigDouble value)
        {
            this.Id = id;
            this.Description = description;
            if (value == null)
            {
                throw new ArgumentException();
            }
            this.value = value;
        }

        public string ValueAsString()
        {
            return IdleEngine.CoerceToString(value);
        }

        public BigDouble ValueAsNumber()
        {
            return IdleEngine.CoerceToNumber(value);
        }

        public abstract object Apply(object input);
    }
}
