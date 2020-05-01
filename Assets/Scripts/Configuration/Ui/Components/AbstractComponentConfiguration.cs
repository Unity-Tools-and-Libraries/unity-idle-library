using IdleFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractComponentConfiguration : UiComponentConfiguration
{
    private readonly string componentId;
    private readonly StateMatcher enabledWhen;
    public string ComponentId => componentId;
    public StateMatcher EnabledWhen => enabledWhen;


    public AbstractComponentConfiguration(string componentId, StateMatcher enabledWhen)
    {
        this.componentId = componentId;
        this.enabledWhen = enabledWhen;
    }
}
