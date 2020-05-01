using IdleFramework.Configuration.UI.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework.Configuration.UI
{
    public class UiConfiguration: UiComponentConfiguration
    {
        private readonly List<TabConfiguration> tabs = new List<TabConfiguration>();

        public List<TabConfiguration> Tabs => tabs;

        public string ComponentId => "root";

        public StateMatcher EnabledWhen => Always.Instance;

        public UiConfiguration WithTab(TabConfiguration tab)
        {
            tabs.Add(tab);
            return this;
        }
    }
}