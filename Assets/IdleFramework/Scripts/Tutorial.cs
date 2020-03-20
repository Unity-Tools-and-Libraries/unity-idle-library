namespace IdleFramework
{
    public class Tutorial : Updates
    {
        private readonly TutorialConfiguration tutorial;
        private bool triggered;

        public Tutorial(TutorialConfiguration tutorial)
        {
            this.triggered = false;
            this.tutorial = tutorial;
        }

        public void Update(IdleEngine engine, float deltaTime)
        {
            if(!triggered && tutorial.TriggerMatcher.Matches(engine))
            {
                triggered = true;
            }
        }
    }
}