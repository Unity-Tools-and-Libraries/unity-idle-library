using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace io.github.thisisnozaku.idle.framework.Events {
    public class ChildValueChangedEvent : ScriptingContext
    {
        public static readonly string EventName = "child_value_changed";
        
        private ValueContainer container;
        private string changedPath;
        private object oldValue;
        private object newValue;
        private string reason;
        public ChildValueChangedEvent(ValueContainer container, string changedPath, object oldValue, object newValue, string reason)
        {
            this.container = container;
            this.changedPath = changedPath;
            this.oldValue = oldValue;
            this.newValue = newValue;
            this.reason = reason;
        }

        public Dictionary<string, object> GetScriptingContext(string contextType = null)
        {
            return new Dictionary<string, object>()
            {

                { "path", changedPath },
                { "old", oldValue },
                { "new", newValue },
                { "reason", reason }
            };
        }
    }
}