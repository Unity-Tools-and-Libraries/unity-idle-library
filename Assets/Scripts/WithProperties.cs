using BreakInfinity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework
{
    public interface WithProperties
    {
        string GetStringProperty(string property);
        bool GetBooleanProperty(string property);
        BigDouble GetNumberProperty(string property);
        Dictionary<string, ValueContainer> GetMapProperty(string property);
        List<ValueContainer> GetListProperty(string property);
    }
}