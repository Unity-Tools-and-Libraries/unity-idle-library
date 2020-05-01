using IdleFramework.UI.Layouts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace IdleFramework.UI.Components
{
    /*
     * Configuration for a Tabs Component
     */
    public class TabConfiguration : PanelConfiguration
    {
        private readonly string componentId;
        private StringContainer tabText = Literal.Of("");

        public TabConfiguration(string componentId) : base(componentId)
        {
            this.componentId = componentId;
        }

        public TabConfiguration WithText(string text)
        {
            tabText = Literal.Of(text);
            return this;
        }

        public StringContainer text => tabText;
    }
}