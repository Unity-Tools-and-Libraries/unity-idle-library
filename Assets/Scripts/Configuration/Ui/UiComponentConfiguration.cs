namespace IdleFramework.Configuration.UI
{
    public interface UiComponentConfiguration
    {
        string ComponentId { get; }
        StateMatcher EnabledWhen { get; }
    }
}