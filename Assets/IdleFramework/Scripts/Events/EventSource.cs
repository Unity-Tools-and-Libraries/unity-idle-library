using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;

public interface EventSource
{
    Dictionary<string, List<Action<object>>> EventListeners { get; }
    void Subscribe(string eventName, Action<object> listener);
}
