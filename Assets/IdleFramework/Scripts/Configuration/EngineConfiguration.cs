using BreakInfinity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace IdleFramework.Configuration
{
    public class EngineConfiguration
    {
        private IDictionary<string, ValueReferenceDefinition> globalProperties = new Dictionary<string, ValueReferenceDefinition>();
        public IDictionary<string, ValueReferenceDefinition> GlobalProperties
        {
            get => new ReadOnlyDictionary<string, ValueReferenceDefinition>(globalProperties);
        }

        public void DeclareGlobalProperty(string propertyName)
        {
            globalProperties[propertyName] = new ValueReferenceDefinitionBuilder()
                .Build();
        }

        public void DeclareGlobalProperty(string property, ValueReferenceDefinition valueReferenceDefinition)
        {
            globalProperties[property] = valueReferenceDefinition;
        }

        public void DeclareGlobalProperty(string propertyName, BigDouble value)
        {
            globalProperties[propertyName] = new ValueReferenceDefinitionBuilder().WithStartingValue(value)
                .Build();
        }
    }
}