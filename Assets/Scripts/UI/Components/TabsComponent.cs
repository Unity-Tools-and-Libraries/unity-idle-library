using UnityEngine;

namespace IdleFramework.UI.Components
{
    public class TabsComponent : MonoBehaviour
    {
        private GameObject tabPrefab;
        public StringContainer[] tabTitles;
        public GameObject[] tabContents;
        public int activeTabIndex = -1;

        private GameObject tabs;
        private GameObject content;

        private IdleEngine engine;

        void Start()
        {
            activeTabIndex = -1;
            tabPrefab = Resources.Load<GameObject>("UI/Component/Prefabs/Tab");
            tabs = transform.Find("Tabs").gameObject;
            content = transform.Find("Scroll View").Find("Viewport").Find("Content").gameObject;
            foreach (var title in tabTitles)
            {
                var newTab = GameObject.Instantiate(tabPrefab, tabs.transform);
                var tabComponent = newTab.GetComponent<TabComponent>();
                tabComponent.tabText = title.Get(engine);
            }
            foreach (var tabContent in tabContents)
            {
                tabContent.transform.SetParent(content.transform, false);
                tabContent.SetActive(false);
            }
            SwitchActiveTab(0);
        }

        // Update is called once per frame
        void Update()
        {

        }

        void SwitchActiveTab(int newActiveTabIndex)
        {
            if (newActiveTabIndex != activeTabIndex)
            {
                if (activeTabIndex >= 0)
                {
                    tabContents[activeTabIndex].transform.SetParent(null);
                    tabContents[activeTabIndex].SetActive(false);
                }
                activeTabIndex = newActiveTabIndex;
                tabContents[activeTabIndex].SetActive(true);
                tabContents[activeTabIndex].transform.SetParent(content.transform);
            }
        }
    }
}