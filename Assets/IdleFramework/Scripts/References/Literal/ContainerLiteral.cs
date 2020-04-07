using BreakInfinity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework
{
    public class ContainerLiteral : LiteralValue
    {
        private readonly PropertyContainer properties;

        public ContainerLiteral(Dictionary<string, ValueContainer> properties)
        {
            this.properties = new SimplePropertyContainer.SimplePropertyContainerBuilder().WithProperties(properties).Build();
        }

        public bool GetAsBoolean(IdleEngine engine)
        {
            return false;
        }

        public PropertyContainer GetAsContainer(IdleEngine engine)
        {
            return properties;
        }

        public BigDouble GetAsNumber(IdleEngine engine)
        {
            return 0;
        }

        public string GetAsString(IdleEngine engine)
        {
            return properties.ToString();
        }

        public object RawValue(IdleEngine engine)
        {
            return properties;
        }
    }
}