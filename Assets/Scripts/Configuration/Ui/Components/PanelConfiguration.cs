using IdleFramework.UI.Layouts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework.UI.Components
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

        public string ComponentId => componentId;
        public LayoutConfigurationBuilder Layout => layout;
        public List<UiComponentConfiguration> Children => children;
    }
}