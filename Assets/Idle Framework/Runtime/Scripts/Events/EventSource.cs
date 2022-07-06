using System.Collections.Generic;
using static io.github.thisisnozaku.idle.framework.ValueContainer;

namespace io.github.thisisnozaku.idle.framework.Events
{
    /**
     * Interface to mark something which generates events.
     */ 
    public interface EventSource
    {
        /*
         * Subscribe to events with the given name on this source.
         */
        ListenerSubscription Subscribe(string subscriber, string eventName, string handler, bool ephemeral = false);
        /*
         * Remove the given subscription.
         */
        void Unsubscribe(ListenerSubscription subscription);
        /*
         * Immediately notify all listeners on this source.
         * 
         * Additionally, will notify parents of this source if the event bubbles.
         */
        void NotifyImmediately(string eventName);
        /*
         * Immediately notify all listeners on this source.
         * 
         * Additionally, will notify parents of this source if the event bubbles, using the given context instead of the containers own.
         */
        void NotifyImmediately(string eventName, ScriptingContext overrideContext);

        /*
         * Immediately notify all listeners on this source.
         * 
         * Additionally, will notify parents of this source if the event bubbles.
         * 
         * * If overrideContext is specified, use that context instead of the containers own.
         */
        void NotifyImmediately(string eventName, string overrideContextName);
    }
}