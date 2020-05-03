using Boo.Lang;
using IdleFramework.Configuration.UI;
using IdleFramework.Configuration.UI.Components;
using System;
using UnityEngine;

namespace IdleFramework.UI.Components.Generators
{
    public class UiGenerator : UiComponentGenerator<UiComponentConfiguration>
    {
        public static readonly UiGenerator Instance = new UiGenerator();
        private readonly TabsComponentGenerator tabsGenerator = new TabsComponentGenerator();
        private readonly TabComponentGenerator tabGenerator = new TabComponentGenerator();
        private readonly PanelComponentGenerator panelGenerator = new PanelComponentGenerator();
        private readonly ButtonComponentGenerator buttonGenerator = new ButtonComponentGenerator();
        private readonly LabelComponentGenerator labelGenerator = new LabelComponentGenerator();
        private readonly ListComponentGenerator listGenerator = new ListComponentGenerator();

        public GameObject Generate(UiComponentConfiguration uiConfiguration, GameObject root, IdleEngine engine)
        {
            if (typeof(UiConfiguration) == uiConfiguration.GetType())
            {
                return tabsGenerator.Generate((UiConfiguration)uiConfiguration, root, engine);
            }
            else if (typeof(PanelConfiguration).IsAssignableFrom(uiConfiguration.GetType()))
            {
                return panelGenerator.Generate((PanelConfiguration)uiConfiguration, root, engine);
            }
            else if (typeof(ButtonConfiguration) == uiConfiguration.GetType())
            {
                return buttonGenerator.Generate((ButtonConfiguration)uiConfiguration, root, engine);
            }
            else if (typeof(LabelConfiguration) == uiConfiguration.GetType())
            {
                return labelGenerator.Generate((LabelConfiguration)uiConfiguration, root, engine);
            }
            else if (typeof(ListConfiguration) == uiConfiguration.GetType())
            {
                return listGenerator.Generate(uiConfiguration as ListConfiguration, root, engine);
            }
            else
            {
                throw new InvalidOperationException(string.Format("No generator for type {0} found", uiConfiguration.GetType()));
            }
        }
    }
}