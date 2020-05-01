using IdleFramework;
using IdleFramework.UI.Layouts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface UiComponentConfiguration
{
    string ComponentId { get; }
    StateMatcher EnabledWhen { get; }
}
