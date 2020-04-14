using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework
{
    public class GlobalBooleanPropertyReference : GlobalPropertyReference, BooleanContainer
    {
        private readonly string propertyName;
        public string PropertyName => propertyName;

        public GlobalBooleanPropertyReference(string propertyName)
        {
            this.propertyName = propertyName;
        }

        public bool Get(IdleEngine engine)
        {
            return engine.GetGlobalBooleanProperty(propertyName).Get(engine);
        }
    }
}