using IdleFramework;

namespace IdleFramework.Configuration.UI.Components
{
    public class LabelConfiguration : AbstractComponentConfiguration
    {
        private readonly ValueContainer value;

        public LabelConfiguration(string componentId, ValueContainer value) : base(componentId, Always.Instance)
        {
            this.value = value;
        }

        public ValueContainer Value => value;
    }
}