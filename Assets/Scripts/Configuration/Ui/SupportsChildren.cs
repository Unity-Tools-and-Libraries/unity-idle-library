namespace IdleFramework.Configuration.UI
{
    public interface SupportsChildren<T> where T : UiComponentConfiguration
    {
        T WithChild(UiComponentConfiguration child);
        LayoutConfigurationBuilder Layout { get; }
    }
}