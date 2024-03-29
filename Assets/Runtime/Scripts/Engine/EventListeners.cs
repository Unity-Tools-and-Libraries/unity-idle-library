﻿using io.github.thisisnozaku.idle.framework.Events;
using io.github.thisisnozaku.scripting.context;
using MoonSharp.Interpreter;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace io.github.thisisnozaku.idle.framework.Engine
{
    public class EventListeners : IEventSource
    {
        public Dictionary<string, Dictionary<string, string>> listeners = new Dictionary<string, Dictionary<string, string>>();
        [JsonIgnore]
        public Dictionary<string, Dictionary<string, CallbackFunction>> callbacks = new Dictionary<string, Dictionary<string, CallbackFunction>>();
        private IdleEngine Engine;
        private bool isRoot;

        public EventListeners(IdleEngine engine, bool isRoot = true)
        {
            this.Engine = engine;
            this.isRoot = isRoot;
        }

        public Dictionary<string, Dictionary<string, string>> GetListeners() => listeners;

        [OnDeserialized]
        public void OnDeserialization(StreamingContext ctx)
        {
            this.Engine = (IdleEngine)ctx.Context;
        }

        public void Emit(string eventName, IDictionary<string, object> contextToUse)
        {
            Engine.Logging.Log(string.Format("Emitting {0}", eventName), "events");
            Dictionary<string, string> eventListenersBySubscriber = null;
            Dictionary<string, CallbackFunction> callbacksBySubscriber = null;
            listeners.TryGetValue(eventName, out eventListenersBySubscriber);
            callbacks.TryGetValue(eventName, out callbacksBySubscriber);
            if(eventListenersBySubscriber != null)
            {
                Engine.Logging.Log(string.Format("Notifying {0} listener(s)", eventListenersBySubscriber.Count));
                lock (eventListenersBySubscriber)
                {
                    foreach (var subscription in eventListenersBySubscriber)
                    {
                        if (subscription.Value != null)
                        {
                            if (callbacksBySubscriber != null && callbacksBySubscriber.ContainsKey(subscription.Key))
                            {
                                throw new InvalidOperationException(String.Format("Subscriber '{0}' for event '{1}' had both a callback and a script to handle it, which is not supported.", subscription.Key, eventName));
                            }
                            Engine.Logging.Log(string.Format("Invoking script listener for subscriber {0}", subscription.Key), "events");
                            Engine.Scripting.EvaluateStringAsScript(subscription.Value, contextToUse);
                        }
                    }
                }
            }
            if(callbacksBySubscriber != null)
            {
                Engine.Logging.Log(string.Format("Notifying {0} listeners", callbacksBySubscriber.Count));
                lock (callbacksBySubscriber)
                {
                    foreach (var subscription in callbacksBySubscriber)
                    {
                        Engine.Logging.Log(string.Format("Invoking callback listener for subscriber {0}", subscription.Key), "events");
                        Engine.Scripting.Evaluate(DynValue.NewCallback(subscription.Value), contextToUse);
                    }
                }
            }
            if(!isRoot)
            {
                Engine.Emit(eventName, contextToUse);
            }
        }

        public void Emit(string eventName, IScriptingContext contextToUse)
        {
            Emit(eventName, contextToUse != null ? contextToUse.GetContextVariables() : null);
        }

        public void Emit(string eventName, Tuple<string, object> contextToUse)
        {
            Emit(eventName, new Dictionary<string, object>()
            {
                {contextToUse.Item1, contextToUse.Item2 }
            });
        }

        public void Watch(string eventName, string subscriber, string handler)
        {
            Engine.Logging.Log(string.Format("Subscriber {0} begun watching {1} with a script", subscriber, eventName), "events");
            Dictionary<string, string> eventListeners = null;
            if (!listeners.TryGetValue(eventName, out eventListeners))
            {
                eventListeners = new Dictionary<string, string>();
                listeners[eventName] = eventListeners;
            }
            lock (eventListeners)
            {
                eventListeners[subscriber] = handler;
            }
            if(EngineReadyEvent.EventName == eventName && Engine.IsReady)
            {
                Engine.Logging.Log("Engine was ready already, so we're invoking immediately", "events");
                Engine.Scripting.EvaluateStringAsScript(handler);
            }
        }

        public void Watch( string eventName, string subscriber, DynValue handler)
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
            Engine.Logging.Log(string.Format("Subscriber {0} begun watching {1} with a callback", subscriber, eventName), "events");
            Dictionary<string, CallbackFunction> callbacks = null;
            if (!this.callbacks.TryGetValue(eventName, out callbacks))
            {
                callbacks = new Dictionary<string, CallbackFunction>();
                this.callbacks[eventName] = callbacks;
            }
            lock (callbacks)
            {
                callbacks[subscriber] = callback;
            }
            if(eventName == EngineReadyEvent.EventName && Engine.IsReady)
            {
                Engine.Scripting.Evaluate(DynValue.NewCallback(callback), Engine);
            }
        }

        public void StopWatching(string eventName, string subscriber)
        {
            Dictionary<string, string> eventListeners = null;
            if (listeners.TryGetValue(eventName, out eventListeners))
            {
                eventListeners[subscriber] = null;
            }
            lock (this.callbacks)
            {
                Dictionary<string, CallbackFunction> callbacks;
                if (this.callbacks.TryGetValue(eventName, out callbacks))
                {
                    callbacks[subscriber] = null;
                }
            }
            Engine.Logging.Log(() => string.Format("Subscriber {0} no longer watching {1}", subscriber, eventName), "events");
        }
    }
}