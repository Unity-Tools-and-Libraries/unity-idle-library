using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace io.github.thisisnozaku.idle.framework.Events
{
    public class ValueChangedEvent : ScriptingContext
    {
        public const string EventName = "value_changed";
        public static readonly List<Tuple<Type, String>> Arguments = new List<Tuple<Type, String>>()
        {
            Tuple.Create(typeof(ValueContainer), "The container whose value changed."),
            Tuple.Create(typeof(object), "The previous raw value."),
            Tuple.Create(typeof(object), "The new raw value"),
            Tuple.Create(typeof(string), "The reason that the change occured: set (the value was set directly), calculated (the value changed from the output of the updater method), restored (value was restored from a snapshot, modifiers (a modifier was added or removed)")
        };
        private ValueContainer changedContainer;
        private object oldValue;
        private object newValue;
        private string reason;

        public ValueChangedEvent(ValueContainer changedContainer, object oldValue, object newValue, string reason)
        {
            this.changedContainer = changedContainer;
            this.oldValue = oldValue;
            this.newValue = newValue;
            this.reason = reason;
        }

        public Dictionary<string, object> GetScriptingContext(string contextType = null)
        {
            return new Dictionary<string, object>()
            {
                { "value", newValue },
                { "previous", oldValue },
                { "reason", reason }
            };
        }
    }
}