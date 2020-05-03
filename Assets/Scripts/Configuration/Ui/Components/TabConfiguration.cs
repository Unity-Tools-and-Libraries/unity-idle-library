namespace IdleFramework.Configuration.UI.Components
{
    /*
     * Configuration for a Tabs Component
     */
    public class TabConfiguration : PanelConfiguration
    {
        private StringContainer tabText = Literal.Of("");

        public TabConfiguration(string componentId, string title) : base(componentId)
        {
            tabText = Literal.Of(title);
        }

        public TabConfiguration(string componentId, StringContainer title) : base(componentId)
        {
            tabText = title;
        }

        public new TabConfiguration WithLayout(LayoutConfigurationBuilder layout)
        {
            return base.WithLayout(layout) as TabConfiguration;
        }

        public new TabConfiguration WithChild(UiComponentConfiguration child)
        {
            return base.WithChild(child) as TabConfiguration;
        }

        public StringContainer text => tabText;


    }
}