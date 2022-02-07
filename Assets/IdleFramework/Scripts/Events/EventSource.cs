using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;

namespace IdleFramework.Events
{
    /**
     * Interface to mark something which generates events.
     */ 
    public interface EventSource
    {
        Dictionary<string, List<Action<object>>> EventListeners { get; }
        void Subscribe(string eventName, Action<object> listener);
    }
}