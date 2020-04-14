using BreakInfinity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework
{
    public class EntityDefinition: IEntityDefinition { 
        private readonly string entityKey;
        private readonly string variantKey;
        private readonly StringContainer name;
        private readonly Dictionary<string, NumberContainer> baseScaledProductionInputs;
        private readonly Dictionary<string, NumberContainer> baseScaledProductionOutputs;
        private readonly Dictionary<string, NumberContainer> baseFixedProductionInputs;
        private readonly Dictionary<string, NumberContainer> baseFixedProductionOutputs;
        private readonly Dictionary<string, NumberContainer> baseRequirements;
        private readonly Dictionary<string, NumberContainer> baseUpkeep;
        private readonly Dictionary<string, NumberContainer> baseCosts;
        private readonly PropertyHolder customProperties = new PropertyHolder();
        private readonly ISet<string> types;
        private readonly StateMatcher isHiddenMatcher;
        private readonly StateMatcher isAvailableMatcher;
        private readonly StateMatcher isEnabledMatcher;
        private readonly NumberContainer quantityCap;
        private readonly BigDouble startingQuantity;
        private readonly NumberContainer calculatedQuantity;
        private readonly bool accumulates;
        private readonly Dictionary<string, EntityDefinition> instances = new Dictionary<string, EntityDefinition>();
        public EntityDefinition(EntityDefinitionBuilder other)
        {
            entityKey = other.EntityKey;
            variantKey = other.VariantKey;
            name = other.Name;
            baseScaledProductionInputs = other.BaseProductionInputs;
            baseScaledProductionOutputs = other.BaseProductionOutputs;
            baseFixedProductionInputs = other.BaseFixedProductionInputs;
            baseFixedProductionOutputs = other.BaseFixedProductionOutputs;
            baseRequirements = other.BaseRequirements;
            baseUpkeep = other.BaseUpkeep;
            baseCosts = other.BaseCosts;
            types = other.Types;
            isHiddenMatcher = other.IsVisibleMatcher;
            isAvailableMatcher = other.IsAvailableMatcher;
            isEnabledMatcher = other.IsEnabledMatcher;
            quantityCap = other.QuantityCap;
            startingQuantity = other.StartingQuantity;
            calculatedQuantity = other.CalculatedQuantity;
            customProperties = other.CustomProperties;
            accumulates = other.Accumulates;
            foreach(var instance in other.Variants)
            {
                instances.Add(instance.Key, instance.Value);
            }
        }

        public object GetInstance(string instanceKey)
        {
            if(!instances.ContainsKey(instanceKey))
            {
                throw new InvalidOperationException(string.Format("No instance with name {0} exists", instanceKey));
            }
            return instances[instanceKey];
        }

        public string EntityKey => entityKey;
        public string VariantKey => variantKey;
        public StringContainer Name => name;
        public BigDouble StartingQuantity => startingQuantity;
        public Dictionary<string, NumberContainer> BaseScaledInputs => baseScaledProductionInputs;
        public Dictionary<string, NumberContainer> BaseRequirements => baseRequirements;
        public Dictionary<string, NumberContainer> BaseCosts => baseCosts;

        public Dictionary<string, NumberContainer> BaseScaledOutputs => baseScaledProductionOutputs;

        public Dictionary<string, NumberContainer> BaseFixedInputs => baseFixedProductionInputs;

        public Dictionary<string, NumberContainer> BaseFixedOutputs => baseFixedProductionOutputs;

        public Dictionary<string, NumberContainer> BaseUpkeep => baseUpkeep;

        public ISet<string> Types => types;
        public Dictionary<string, EntityDefinition> Variants => instances;

        public StateMatcher IsVisibleMatcher => isHiddenMatcher;

        public StateMatcher IsAvailableMatcher => isAvailableMatcher;

        public StateMatcher IsEnabledMatcher => isEnabledMatcher;

        public NumberContainer QuantityCap => quantityCap;

        public NumberContainer CalculatedQuantity => calculatedQuantity;

        public PropertyHolder CustomProperties => customProperties;
        
        public bool Accumulates => accumulates;

        internal object GetCustomStringProperty(string key)
        {
            throw new NotImplementedException();
        }

        internal void SetCustomProperty(string key, StringContainer value)
        {
            throw new NotImplementedException();
        }

        internal object GetCustomBooleanProperty(string key)
        {
            throw new NotImplementedException();
        }

        internal object GetCustomNumberProperty(string key)
        {
            throw new NotImplementedException();
        }

        internal void SetCustomProperty(string key, BooleanContainer value)
        {
            throw new NotImplementedException();
        }

        internal void SetCustomProperty(string key, NumberContainer value)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return string.Format("Definition for entity({0}), variant({1})", entityKey, variantKey);
        }
    }
}