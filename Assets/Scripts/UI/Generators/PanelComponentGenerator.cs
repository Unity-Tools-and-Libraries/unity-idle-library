using IdleFramework.UI.Layouts;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

namespace IdleFramework.UI.Components.Generators
{
    public class PanelComponentGenerator : UiComponentGenerator<PanelConfiguration>
    {
        public GameObject Generate(PanelConfiguration uiConfiguration, GameObject parent, IdleEngine engine)
        {
            var instantiatedPanel = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("UI/Component/Prefabs/Panel"), parent.transform, false);
            instantiatedPanel.name = uiConfiguration.ComponentId + "-panel";
            if (uiConfiguration.Layout != null)
            {
                AddLayout(instantiatedPanel, uiConfiguration.Layout);
            }
            foreach(var childConfig in uiConfiguration.Children)
            {
                UiGenerator.Instance.Generate(childConfig, instantiatedPanel, engine);
            }
            var rt = instantiatedPanel.transform as RectTransform;
            rt.anchorMax = Vector3.one;
            rt.anchoredPosition = Vector3.zero;
            return instantiatedPanel;
        }

        private void AddLayout(GameObject gameObject, LayoutConfigurationBuilder configuration)
        {
            switch (configuration.Direction)
            {
                case LayoutDirection.HORIZONTAL:
                    {
                        var layout = gameObject.AddComponent<HorizontalLayoutGroup>();
                        break;
                    }
                case LayoutDirection.VERTICAL:
                    {
                        var layout = gameObject.AddComponent<VerticalLayoutGroup>();
                        break;
                    }
            }
        }
    }
}