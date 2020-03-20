using BreakInfinity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework
{
    public class GlobalPropertyReference : PropertyReference
    {
        private readonly string propertyName;

        public string PropertyName => propertyName;

        public GlobalPropertyReference(string propertyName)
        {
            this.propertyName = propertyName;
        }

        public BigDouble GetAsNumber(IdleEngine engine)
        {
            return engine.GetGlobalProperty(propertyName);
        }
    }
}