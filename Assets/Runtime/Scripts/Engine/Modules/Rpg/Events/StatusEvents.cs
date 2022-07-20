using BreakInfinity;
using io.github.thisisnozaku.idle.framework;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg.Events;
using io.github.thisisnozaku.idle.framework.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg
{
    /*
     * Event emitted
     */
    public class StatusAddedEvent : RpgCharacterModifierEvent<CharacterStatus>
    {
        public const string EventName = "statusAdded";
        private CharacterStatus status;
        private BigDouble duration;

        public StatusAddedEvent(CharacterStatus status, BigDouble duration)
        {
            this.status = status;
            this.duration = duration;
        }

        public override Dictionary<string, object> GetScriptingProperties()
        {
            return new Dictionary<string, object>()
            {
                { "status", status },
                { "duration", duration }
            };
        }
    }

    public class StatusRemovedEvent : RpgCharacterModifierEvent<CharacterStatus>
    {
        public const string EventName = "statusRemoved";
        private CharacterStatus status;

        public StatusRemovedEvent(CharacterStatus status)
        {
            this.status = status;
        }

        public override Dictionary<string, object> GetScriptingProperties()
        {
            return new Dictionary<string, object>()
            {
                { "status", status }
            };
        }
    }
}