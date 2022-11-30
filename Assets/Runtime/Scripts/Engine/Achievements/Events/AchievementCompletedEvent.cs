using io.github.thisisnozaku.scripting.context;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace io.github.thisisnozaku.idle.framework.Engine.Achievements.Events
{
    public class AchievementCompletedEvent : IScriptingContext
    {
        public const string EventName = "achievementCompleted";
        public Achievement Achievement { get; }

        public AchievementCompletedEvent(Achievement achievement)
        {
            this.Achievement = achievement;
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