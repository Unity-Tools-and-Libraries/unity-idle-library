using IdleFramework;

public class LabelConfiguration : AbstractComponentConfiguration
{
    private readonly string componentId;
    private readonly ValueContainer value;

    public LabelConfiguration(string componentId, ValueContainer value): base(componentId, Always.Instance)
    {
        this.componentId = componentId;
        this.value = value;
    }

    public string ComponentId => componentId;

    public ValueContainer Value => value;
}
