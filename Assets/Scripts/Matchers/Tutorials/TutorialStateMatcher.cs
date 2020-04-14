namespace IdleFramework
{
    public abstract class TutorialStateMatcher : StateMatcher
    {
        private string achievementKey;

        protected TutorialStateMatcher(string achievementKey)
        {
            this.achievementKey = achievementKey;
        }

        public string AchievementKey { get => achievementKey; set => achievementKey = value; }

        public abstract bool Matches(IdleEngine toCheck);
    }
}