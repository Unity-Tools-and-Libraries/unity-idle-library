using BreakInfinity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework
{
    public interface PropertyContainer
    {
        ValueContainer Get(string propertyName);
        void SetProperty(string propertyName, ValueContainer value);
        IEnumerable<KeyValuePair<string, ValueContainer>> Properties { get; }
    }
}