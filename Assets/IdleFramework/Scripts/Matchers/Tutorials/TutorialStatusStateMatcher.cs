using System;
namespace IdleFramework
{
    public class TutorialStatusStateMatcher : TutorialStateMatcher
    {
        private readonly Tutorial.Status status;
        public TutorialStatusStateMatcher(string achievementKey, Tutorial.Status status) : base(achievementKey)
        {
            if(AchievementKey == null)
            {
                throw new InvalidOperationException("achievementKey must not be null.");
            }
            this.status = status;
        }

        public override bool Matches(IdleEngine engine)
        {
            return engine.Tutorials[this.AchievementKey].Progress >= status;
        }
    }
}