namespace IdleFramework.Configuration.UI.Components
{
    /*
     * Configuration for a Tabs Component
     */
    public class TabConfiguration : PanelConfiguration
    {
        private StringContainer tabText = Literal.Of("");

        public TabConfiguration(string componentId) : base(componentId)
        {
            
        }

        public TabConfiguration WithText(string text)
        {
            tabText = Literal.Of(text);
            return this;
        }

        public StringContainer text => tabText;
    }
}