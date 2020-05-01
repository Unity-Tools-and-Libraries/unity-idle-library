using IdleFramework.UI.Layouts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework.UI.Components
{
    public interface SupportsChildren<T> where T : UiComponentConfiguration
    {
        T WithChild(UiComponentConfiguration child);
        LayoutConfigurationBuilder Layout { get; }
    }
}