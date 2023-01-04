using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace io.github.thisisnozaku.idle.framework.Engine.State
{
    public class StateChangedEvent : StateMachineEvent
    {
        public const string EventName = "state_changed";
        public string PreviousState { get; }
        public string NewState { get; }

        public StateChangedEvent(string newState, string previousState = "")
        {
            PreviousState = previousState;
            NewState = newState;
        }

        public Dictionary<string, object> GetContextVariables()
        {
            return new Dictionary<string, object>()
            {
                { "ev", this }
            };
        }
    }
}