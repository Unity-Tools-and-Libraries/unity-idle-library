using UnityEngine;

namespace IdleFramework.UI.Components
{
    public class TabsComponent : MonoBehaviour
    {
        public StringContainer[] tabTitles;
        public  GameObject[] tabContents;
        public int activeTabIndex = 0;

        private GameObject tabs;
        private GameObject content;
        
        void Start()
        {
            tabs = transform.Find("Tabs").gameObject;
            content = transform.Find("Scroll View").Find("Viewport").Find("Content").gameObject;
            SwitchActiveTab(0);
        }

        // Update is called once per frame
        void Update()
        {

        }

        void SwitchActiveTab(int newActiveTabIndex)
        {
            tabContents[activeTabIndex].transform.SetParent(null);
            tabContents[activeTabIndex].SetActive(false);
            activeTabIndex = newActiveTabIndex;
            tabContents[activeTabIndex].SetActive(true);
            tabContents[activeTabIndex].transform.SetParent(content.transform);
        }
    }
}