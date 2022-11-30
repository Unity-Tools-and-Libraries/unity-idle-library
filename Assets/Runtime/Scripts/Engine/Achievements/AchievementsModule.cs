using io.github.thisisnozaku.idle.framework.Engine.Achievements.Events;
using MoonSharp.Interpreter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace io.github.thisisnozaku.idle.framework.Engine.Achievements
{
    public class AchievementsModule: IEnumerable<KeyValuePair<long, Achievement>>
    {
        private Dictionary<long, Achievement> achievements = new Dictionary<long, Achievement>();
        public Achievement this[long id]
        {
            get {
                Achievement achievement;
                if(achievements.TryGetValue(id, out achievement))
                {
                    return achievement;
                }
                return null;
            }
            set
            {
                achievements[id] = value;
            }
        }

        public AchievementsModule()
        {
            UserData.RegisterType<Achievement>();
            UserData.RegisterType<AchievementCompletedEvent>();
        }

        public IEnumerator<KeyValuePair<long, Achievement>> GetEnumerator()
        {
            return achievements.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return achievements.GetEnumerator();
        }

        public Dictionary<long, Achievement>.ValueCollection Values => achievements.Values;
    }
}