using io.github.thisisnozaku.idle.framework.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace io.github.thisisnozaku.idle.framework.Engine
{
    public class EventListeners : IEventSource
    {
        public Dictionary<string, Dictionary<string, string>> listeners = new Dictionary<string, Dictionary<string, string>>();
        private IdleEngine engine;
        private int notificationCount = 0;
        public EventListeners(IdleEngine engine)
        {
            this.engine = engine;
        }

        public Dictionary<string, Dictionary<string, string>> GetListeners() => listeners;

        public void Emit(string eventName, IDictionary<string, object> contextToUse)
        {
            Dictionary<string, string> eventListenersBySubscriber = null;
            if (listeners.TryGetValue(eventName, out eventListenersBySubscriber))
            {
                var toIterate = eventListenersBySubscriber.Values.Where(l => l != null).ToArray();
                foreach (var listener in toIterate)
                {
                    engine.Scripting.Evaluate(listener, contextToUse);
                }
            }
        }

        public void Emit(string eventName, ScriptingContext contextToUse)
        {
            Emit(eventName, contextToUse.GetScriptingProperties());
        }

        public void Watch(string eventName, string subscriber, string handler)
        {
            Dictionary<string, string> eventListeners = null;
            if (!listeners.TryGetValue(eventName, out eventListeners))
            {
                eventListeners = new Dictionary<string, string>();
                listeners[eventName] = eventListeners;
            }
            eventListeners[subscriber] = handler;
        }

        public void StopWatching(string eventName, string subscriber)
        {
            Dictionary<string, string> eventListeners = null;
            if (!listeners.TryGetValue(eventName, out eventListeners))
            {
                listeners[eventName] = eventListeners = new Dictionary<string, string>();
            }
            eventListeners[subscriber] = null;
        }
    }
}