using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace io.github.thisisnozaku.idle.framework.Events
{
    public interface IdleEngineEvent
    {
        public bool PreventBubbling { get; set; }
    }

    public class DummyEvent : IdleEngineEvent
    {
        public static IdleEngineEvent Instance = new DummyEvent();
        private DummyEvent() { }
        public bool PreventBubbling { get; set; }
    }
}