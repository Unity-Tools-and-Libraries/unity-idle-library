using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework
{

    /*
     * Action to set a global property.
     */
    public class ModifyGlobalPropertyBaseValue : Action
    {
        private readonly ValueContainer newBaseValue;
        private readonly string propertyName;

        public ModifyGlobalPropertyBaseValue(ValueContainer newBaseValue, string propertyName)
        {
            this.newBaseValue = newBaseValue;
            this.propertyName = propertyName;
        }

        public ValueContainer NewBaseValue => newBaseValue;

        public string PropertyName => propertyName;
    }
}