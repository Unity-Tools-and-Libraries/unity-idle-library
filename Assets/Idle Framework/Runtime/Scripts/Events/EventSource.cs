using System.Collections.Generic;
using static io.github.thisisnozaku.idle.framework.ValueContainer;

namespace io.github.thisisnozaku.idle.framework.Events
{
    /**
     * Interface to mark something which generates events.
     */ 
    public interface EventSource<T>
    {
        /*
         * Subscribe to events with the given name on this source.
         */
        T Subscribe(string subscriber, string eventName, string handlerName, bool ephemeral = false);
        /*
         * Remove the given subscription.
         */
        void Unsubscribe(ListenerSubscription subscription);
        /*
         * Remove the given subscription.
         */
        void Unsubscribe(string subscriber, string eventName);
        /*
         * Immediately notify all listeners on this source.
         * 
         * Additionally, will notify parents of this source if the event bubbles.
         */
        void NotifyImmediately(string eventName, IDictionary<string, object> notificationContext = null, params object[] arguments);

        /*
         * Immediately notify all listeners on this source.
         * 
         * Additionally, will notify parents of this source if the event bubbles.
         */
        void NotifyImmediately(string eventName, params object[] arguments);
    }
}