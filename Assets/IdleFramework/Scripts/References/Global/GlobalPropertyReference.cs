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
            return engine.GetGlobalNumberProperty(propertyName);
        }

        public bool GetAsBoolean(IdleEngine engine)
        {
            return engine.GetGlobalBooleanProperty(propertyName);
        }

        public string GetAsString(IdleEngine engine)
        {
            return engine.GetGlobalStringProperty(propertyName);
        }

        public object RawValue(IdleEngine engine)
        {
            return engine.GetRawGlobalProperty(propertyName);
        }

        public PropertyContainer GetAsContainer(IdleEngine engine)
        {
            throw new System.NotImplementedException();
        }
    }
}