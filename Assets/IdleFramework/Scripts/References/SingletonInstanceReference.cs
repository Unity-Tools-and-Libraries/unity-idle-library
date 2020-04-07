using BreakInfinity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework
{
    public class SingletonReference : ValueContainer
    {
        private readonly string singletonType;
        private readonly string instanceKey;

        public SingletonReference(string singletonType, string instanceKey)
        {
            this.singletonType = singletonType;
            this.instanceKey = instanceKey;
        }

        public string SingletonType => singletonType;

        public string InstanceKey => instanceKey;

        public bool GetAsBoolean(IdleEngine engine)
        {
            throw new System.NotImplementedException();
        }

        public PropertyContainer GetAsContainer(IdleEngine engine)
        {
            throw new System.NotImplementedException();
        }

        public BigDouble GetAsNumber(IdleEngine engine)
        {
            throw new System.NotImplementedException();
        }

        public string GetAsString(IdleEngine engine)
        {
            throw new System.NotImplementedException();
        }

        public object RawValue(IdleEngine engine)
        {
            throw new System.NotImplementedException();
        }
    }
}