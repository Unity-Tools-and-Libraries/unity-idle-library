using BreakInfinity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework
{
    public class GlobalNumberPropertyReference : GlobalPropertyReference, NumberContainer
    {
        private readonly string propertyName;
        public string PropertyName => propertyName;
        public GlobalNumberPropertyReference(string propertyName)
        {
            this.propertyName = propertyName;
        }

        public BigDouble Get(IdleEngine engine)
        {
            return engine.GetGlobalNumberProperty(propertyName).Get(engine);
        }
    }
}