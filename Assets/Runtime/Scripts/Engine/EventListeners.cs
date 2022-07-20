using io.github.thisisnozaku.idle.framework.Events;
using MoonSharp.Interpreter;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace io.github.thisisnozaku.idle.framework.Engine
{
    public class EventListeners : IEventSource
    {
        public Dictionary<string, Dictionary<string, string>> listeners = new Dictionary<string, Dictionary<string, string>>();
        [JsonIgnore]
        public Dictionary<string, Dictionary<string, CallbackFunction>> callbacks = new Dictionary<string, Dictionary<string, CallbackFunction>>();
        private IdleEngine engine;
        public EventListeners(IdleEngine engine)
        {
            this.engine = engine;
        }

        public Dictionary<string, Dictionary<string, string>> GetListeners() => listeners;

        public void Emit(string eventName, IDictionary<string, object> contextToUse)
        {
            Dictionary<string, string> eventListenersBySubscriber = null;
            Dictionary<string, CallbackFunction> callbacksBySubscriber = null;
            listeners.TryGetValue(eventName, out eventListenersBySubscriber);
            callbacks.TryGetValue(eventName, out callbacksBySubscriber);
            if(eventListenersBySubscriber != null)
            {
                foreach(var subscription in eventListenersBySubscriber)
                {
                    if (subscription.Value != null)
                    {
                        if (callbacksBySubscriber != null && callbacksBySubscriber.ContainsKey(subscription.Key))
                        {
                            throw new InvalidOperationException(String.Format("Subscriber '{0}' for event '{1}' had both a callback and a script to handle it, which is not supported.", subscription.Key, eventName));
                        }
                        engine.Scripting.EvaluateString(subscription.Value, contextToUse);
                    }
                }
            }
            if(callbacksBySubscriber != null)
            {
                foreach(var subscription in callbacksBySubscriber)
                {
                    engine.Scripting.ExecuteCallback(subscription.Value, contextToUse);
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
            if(EngineReadyEvent.EventName == eventName)
            {
                engine.Scripting.EvaluateString(handler);
            }
        }

        public void Watch(string eventName, string subscriber, DynValue handler)
        {
            switch(handler.Type)
            {
                case DataType.ClrFunction:
                    Watch(eventName, subscriber, handler.Callback);
                    break;
                case DataType.String:
                    Watch(eventName, subscriber, handler.String);
                    break;
                default:
                    throw new ArgumentException("Handler must be a DynValue containing a string or Clr Function.");
            }
        }

        private void Watch(string eventName, string subscriber, CallbackFunction callback)
        {
            Dictionary<string, CallbackFunction> callbacks = null;
            if (!this.callbacks.TryGetValue(eventName, out callbacks))
            {
                callbacks = new Dictionary<string, CallbackFunction>();
                this.callbacks[eventName] = callbacks;
            }
            callbacks[subscriber] = callback;
            if(eventName == EngineReadyEvent.EventName)
            {
                engine.Scripting.ExecuteCallback(callback);
            }
        }

        public void StopWatching(string eventName, string subscriber)
        {
            Dictionary<string, string> eventListeners = null;
            if (listeners.TryGetValue(eventName, out eventListeners))
            {
                eventListeners[subscriber] = null;
            }
            
            Dictionary<string, CallbackFunction> callbacks;
            if(this.callbacks.TryGetValue(eventName, out callbacks))
            {
                callbacks[subscriber] = null;
            }
        }
    }
}