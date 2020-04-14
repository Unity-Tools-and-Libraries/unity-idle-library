using UnityEngine;

namespace IdleFramework
{
    public class GlobalStringPropertyReference : StringContainer, GlobalPropertyReference
    {
        private readonly string propertyName;
        public string PropertyName => propertyName;
        public GlobalStringPropertyReference(string propertyName)
        {
            this.propertyName = propertyName;
        }

        public string Get(IdleEngine engine)
        {
            return engine.GetGlobalStringProperty(propertyName).Get(engine);
        }
    }
}