using System.Collections.Generic;

namespace IdleFramework.Configuration.UI.Components
{
    public class PanelConfiguration : AbstractComponentConfiguration
    {
        private readonly string componentId;
        protected LayoutConfigurationBuilder layout;
        protected List<UiComponentConfiguration> children = new List<UiComponentConfiguration>();

        public PanelConfiguration(string componentId): base(componentId, Always.Instance)
        {
            this.componentId = componentId;
        }

        public PanelConfiguration WithLayout(LayoutConfigurationBuilder layout)
        {
            this.layout = layout;
            return this;
        }

        public PanelConfiguration WithChild(UiComponentConfiguration child)
        {
            children.Add(child);
            return this;
        }
        public LayoutConfigurationBuilder Layout => layout;
        public List<UiComponentConfiguration> Children => children;
    }
}