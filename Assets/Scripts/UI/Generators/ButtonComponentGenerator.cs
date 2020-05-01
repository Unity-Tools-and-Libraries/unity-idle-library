using IdleFramework.Configuration.UI.Components;
using UnityEngine;

namespace IdleFramework.UI.Components.Generators
{
    public class ButtonComponentGenerator : UiComponentGenerator<ButtonConfiguration>
    {
        public GameObject Generate(ButtonConfiguration uiConfiguration, GameObject parent, IdleEngine engine)
        {
            var instantiatedButton = GameObject.Instantiate(Resources.Load<GameObject>("UI/Component/Prefabs/Button"), parent.transform, false);
            instantiatedButton.name = uiConfiguration.ComponentId + "-button";
            var buttonComponent = instantiatedButton.GetComponent<ButtonComponent>();
            var buttonField = buttonComponent.GetType().GetField("engine", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            buttonField.SetValue(buttonComponent, engine);
            var enabledField = buttonComponent.GetType().GetField("enabledWhen", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            enabledField.SetValue(buttonComponent, uiConfiguration.EnabledWhen);

            buttonComponent.text = uiConfiguration.ButtonText;

            buttonComponent.onClickActions = uiConfiguration.OnClickActions;

            return instantiatedButton;
        }
    }
}