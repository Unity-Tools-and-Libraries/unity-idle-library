using IdleFramework;
using IdleFramework.UI.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface UiComponentGenerator<T> where T : UiComponentConfiguration
{
    GameObject Generate(T uiConfiguration, GameObject parent, IdleEngine engine);
}
