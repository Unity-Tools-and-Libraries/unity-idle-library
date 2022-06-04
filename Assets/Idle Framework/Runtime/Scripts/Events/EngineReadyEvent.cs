using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace io.github.thisisnozaku.idle.framework.Events
{
    public class EngineReadyEvent : IdleEngineEvent
    {
        public bool PreventBubbling { get; set; }
    }
}