using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace io.github.thisisnozaku.idle.framework.Events
{
    public class ValueChangedEvent : ScriptingContext
    {
        public const string EventName = "value_changed";
        
        private string changedPath;
        private object oldValue;
        private object newValue;
        private string reason;

        public ValueChangedEvent(string changedPath, object oldValue, object newValue, string reason)
        {
            this.changedPath = changedPath;
            this.oldValue = oldValue;
            this.newValue = newValue;
            this.reason = reason;
        }

        public Dictionary<string, object> GetScriptingProperties()
        {
            return new Dictionary<string, object>()
            {
                { "path", changedPath },
                { "value", newValue },
                { "previous", oldValue },
                { "reason", reason }
            };
        }
    }
}