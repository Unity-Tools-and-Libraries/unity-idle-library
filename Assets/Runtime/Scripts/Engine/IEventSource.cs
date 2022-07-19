﻿using io.github.thisisnozaku.idle.framework.Events;
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
        void Emit(string eventName, ScriptingContext contextToUse);
        /*
         * This source emits the given event, evaluating the attached listener scripts using the given context.
         */
        void Emit(string eventName, IDictionary<string, object> contextToUse = null);

        /*
         * Watch this source, executing the given handler script when the specified event occurs.
         * 
         * The subscription identifier identifies this subscription for 
         */
        void Watch(string eventName, string watcherIdentifier, string handler);
        /*
         * Unsubscribe from this, causing the script identified by the subscription identifier for the event to not be invoked in the future.
         */
        void StopWatching(string eventName, string subscriptionIdentifier);
    }
}