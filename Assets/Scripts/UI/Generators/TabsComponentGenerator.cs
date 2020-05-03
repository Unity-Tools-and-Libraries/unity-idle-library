using UnityEngine;
using System.Linq;
using IdleFramework.Configuration.UI;

namespace IdleFramework.UI.Components.Generators
{
    public class TabsComponentGenerator : UiComponentGenerator<UiConfiguration>
    {
        public GameObject Generate(UiConfiguration uiConfiguration, GameObject parent, IdleEngine engine)
        {
            var loadedPrefab = Resources.Load<GameObject>("UI/Component/Prefabs/Tabs Component");
            GameObject tabsComponent = GameObject.Instantiate(loadedPrefab, parent.transform, false);
            var tabsComponentConfiguration = tabsComponent.GetComponent<TabsComponent>();
            var viewport = tabsComponent.transform.Find("Scroll View").Find("Viewport").Find("Content").gameObject;
            var tabComponents = uiConfiguration.Tabs.Select(t => {
                return UiGenerator.Instance.Generate(t, viewport, engine);
                });

            tabsComponentConfiguration.tabTitles = uiConfiguration.Tabs.Select(tc => tc.text).ToArray();
            tabsComponentConfiguration.tabContents = tabComponents.ToArray();

            typeof(TabsComponent).GetField("engine", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).SetValue(tabsComponentConfiguration, engine);
            
            return tabsComponent;
        }
    }
}