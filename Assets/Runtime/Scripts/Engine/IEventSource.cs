using io.github.thisisnozaku.idle.framework.Events;
using io.github.thisisnozaku.scripting.context;
using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;

namespace io.github.thisisnozaku.idle.framework.Engine
{
    /*
     * Interface for something which emits events.
     */
    public interface IEventSource
    {
        /*
         * This source emits the given event, evaluating the attached listener scripts using the given context.
         */
        void Emit(string eventName, IScriptingContext contextToUse = null);
        /*
         * This source emits the given event, evaluating the attached listener scripts using the given context.
         */
        void Emit(string eventName, IDictionary<string, object> contextToUse);
        /*
         * This source emits the given event, evaluating the attached listener scripts using the given context.
         */
        void Emit(string eventName, Tuple<string, object> contextToUse);
        /*
         * Watch this source, executing the given handler script when the specified event occurs.
         * 
         * The subscription identifier identifies this subscription for unsubscribing.
         */
        void Watch(string eventName, string watcherIdentifier, string handler);
        /*
         * Watch this source, execuing the callback or string script contained within the DynValue.
         * 
         * The subscription identifier identified this subscription for unsubscribing.
         */
        void Watch(string eventName, string subscriber, DynValue handler);
        /*
         * Unsubscribe from this, causing the script identified by the subscription identifier for the event to not be invoked in the future.
         */
        void StopWatching(string eventName, string subscriptionIdentifier);
    }
}