using BreakInfinity;
using System;
using System.Collections.Generic;

namespace IdleFramework
{
    /*
     * A state comparison matcher that checks that the specified entity exists and compares the value it has for the specified property and sub property.
     */
    public class EntityPropertyMatcher : EntityStateMatcher
    {
        private string entityProperty;
        private string entitySubproperty;
        private Comparison comparison;
        private PropertyReference valueSource;

        private EntityPropertyMatcher(string entityKey, string entityProperty, string entitySubproperty, Comparison comparison) : base(entityKey)
        {
            this.entityProperty = entityProperty.ToLower();
            this.entitySubproperty = entitySubproperty.ToLower();
            this.comparison = comparison;
        }
        public EntityPropertyMatcher(string entityKey, string entityProperty, string entitySubproperty, Comparison comparison, BigDouble value) : this(entityKey, entityProperty, entitySubproperty, comparison)
        {
            this.valueSource = Literal.Of(value);
        }
        public EntityPropertyMatcher(string entityKey, string entityProperty, Comparison comparison, BigDouble value) : this(entityKey, entityProperty, "", comparison, value)
        {

        }

        public EntityPropertyMatcher(string entityKey, string entityProperty, Comparison comparison, PropertyReference reference) : this(entityKey, entityProperty, "", comparison)
        {
            this.valueSource = reference;
        }

        public EntityPropertyMatcher(string entityKey, string entityProperty, string entitySubproperty, Comparison comparison, PropertyReference reference) : this(entityKey, entityProperty, entitySubproperty, comparison)
        {
            this.valueSource = reference;
        }

        

        public override bool Matches(IdleEngine toCheck)
        {
            GameEntity entityToCheck = null;
            if(toCheck.AllEntities.TryGetValue(EntityKey, out entityToCheck))
            {
                var entityValue = getNumberPropertyValue(entityToCheck, entityProperty, entitySubproperty, toCheck);
                return performComparison(entityValue, toCheck);
            }
            return false;
        }

        private BigDouble getNumberPropertyValue(GameEntity entity, string property, string subproperty, IdleEngine toCheck)
        {
            BigDouble numberValue = 0;
            switch(property)
            {
                case "requirements":
                    if(entity.Requirements.ContainsKey(subproperty))
                    {
                        numberValue = entity.Requirements[subproperty].Value;
                    } else if (entity.BaseRequirements.ContainsKey(subproperty))
                    {
                        numberValue = entity.BaseRequirements[subproperty].Get(toCheck);
                    }
                    else
                    {
                        numberValue = 0;
                    }
                    break;
                case "costs":
                    if (entity.Costs.ContainsKey(subproperty))
                    {
                        numberValue = entity.Costs[subproperty].Value;
                    }
                    else if (entity.BaseCosts.ContainsKey(subproperty))
                    {
                        numberValue = entity.BaseCosts[subproperty].Get(toCheck);
                    }
                    else
                    {
                        numberValue = 0;
                    }
                    break;
                case "inputs":
                    if (entity.ProductionInputs.ContainsKey(subproperty))
                    {
                        numberValue = entity.ProductionInputs[subproperty].Value;
                    }
                    else if (entity.BaseProductionInputs.ContainsKey(subproperty))
                    {
                        numberValue = entity.BaseProductionInputs[subproperty].Get(toCheck);
                    }
                    else
                    {
                        numberValue = 0;
                    }
                    break;
                case "outputs":
                    if (entity.ProductionOutputs.ContainsKey(subproperty))
                    {
                        numberValue = entity.ProductionOutputs[subproperty].Value;
                    }
                    else if (entity.BaseProductionOutputs.ContainsKey(subproperty))
                    {
                        numberValue = entity.BaseProductionOutputs[subproperty].Get(toCheck);
                    } else
                    {
                        numberValue = 0;
                    }
                    break;
                case "quantity":
                    numberValue = entity.Quantity;
                    break;
            }
            return numberValue;
        }

        private bool performComparison(BigDouble entityValue, IdleEngine engine)
        {
            switch(comparison)
            {
                case Comparison.EQUALS:
                    return entityValue.CompareTo(this.valueSource.Get(engine)) == 0;
                case Comparison.GREATER_THAN:
                    return entityValue.CompareTo(this.valueSource.Get(engine)) > 0;
                case Comparison.LESS_THAN:
                    return entityValue.CompareTo(this.valueSource.Get(engine)) < 0;
            }
            return false;
        }
    }       

}