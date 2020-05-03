using IdleFramework;

namespace IdleFramework.Configuration.UI.Components
{
    public class LabelConfiguration : AbstractComponentConfiguration
    {
        private readonly ValueContainer value;

        public LabelConfiguration(ValueContainer value) : base("", Always.Instance)
        {
            this.value = value;
        }

        public LabelConfiguration(string value) : this(Literal.Of(value))
        {
            
        }

        public ValueContainer Value => value;
    }
}