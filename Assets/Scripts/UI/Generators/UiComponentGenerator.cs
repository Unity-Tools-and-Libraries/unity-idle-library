using IdleFramework.Configuration.UI;
using UnityEngine;

namespace IdleFramework.UI.Components.Generators
{
    public interface UiComponentGenerator<T> where T : UiComponentConfiguration
    {
        GameObject Generate(T uiConfiguration, GameObject parent, IdleEngine engine);
    }
}