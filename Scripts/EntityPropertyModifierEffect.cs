using BreakInfinity;
using IdleFramework;
using System;

namespace IdleFramework
{
    public class EntityPropertyModifierEffect : EntityEffect
    {
        private string entityKey;
        private string entityProperty;
        private string entitySubProperty;
        private BigDouble value;
        private EffectType type;

        public string EntityProperty { get => entityProperty; }
        public BigDouble Value { get => value; }
        public EffectType Type { get => type; }
        public string EntitySubProperty { get => entitySubProperty; }
        public string EntityKey { get => entityKey; }

        public EntityPropertyModifierEffect(string entityKey, string entityProperty, string entitySubProperty, BigDouble value, EffectType type)
        {
            this.entityKey = entityKey;
            this.entityProperty = entityProperty;
            this.entitySubProperty = entitySubProperty;
            this.value = value;
            this.type = type;
        }

        public override void ApplyEffect(IdleEngine engine)
        {
            GameEntity entity = null;
            if(engine.AllEntities.TryGetValue(EntityKey, out entity))
            {
                ApplyEffectToEntity(entity, engine);
            }
            
        }

        protected void ApplyEffectToEntity(GameEntity entity, IdleEngine engine)
        {
            BigDouble newValue = calculateValue(getBaseValue(entity, engine));
            switch (entityProperty)
            {
                case "inputs":
                    entity.ProductionInputs[entitySubProperty] = newValue;
                    break;
                case "outputs":
                    entity.ProductionOutputs[entitySubProperty] = newValue;
                    break;
                case "requirements":
                    entity.Requirements[entitySubProperty] = newValue;
                    break;
                case "costs":
                    entity.Costs[entitySubProperty] = newValue;
                    break;
                case "upkeep":
                    entity.Upkeep[entitySubProperty] = newValue;
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }

        private BigDouble getBaseValue(GameEntity entity, IdleEngine engine)
        {
            switch(entityProperty)
            {
                case "outputs":
                    if(entity.ProductionOutputs.ContainsKey(entitySubProperty))
                    {
                        return entity.ProductionOutputs[entitySubProperty];
                    } else if (entity.BaseProductionOutputs.ContainsKey(entitySubProperty))
                    {
                        return entity.BaseProductionOutputs[entitySubProperty].Get(engine);
                    }
                    return 0;
                case "inputs":
                    if (entity.ProductionInputs.ContainsKey(entitySubProperty))
                    {
                        return entity.ProductionInputs[entitySubProperty];
                    }
                    else if (entity.BaseProductionInputs.ContainsKey(entitySubProperty))
                    {
                        return entity.BaseProductionInputs[entitySubProperty].Get(engine);
                    }
                    return 0;
                case "upkeep":
                    if (entity.Upkeep.ContainsKey(entitySubProperty))
                    {
                        return entity.Upkeep[entitySubProperty];
                    }
                    else if (entity.BaseUpkeep.ContainsKey(entitySubProperty))
                    {
                        return entity.BaseUpkeep[entitySubProperty].Get(engine);
                    }
                    return 0;
                default:
                    throw new InvalidOperationException();
            }
        }

        private BigDouble calculateValue(BigDouble baseValue)
        {
            switch (type)
            {
                case EffectType.ADD:
                    return baseValue + value;
                case EffectType.SUBTRACT:
                    return baseValue - value;
                case EffectType.MULTIPLY:
                    return baseValue * value;
                default:
                    throw new InvalidOperationException();
            }
        }

        protected bool entityHasPropertyAndSubproperty(GameEntity entity, string entityProperty, string entitySubProperty)
        {
            switch(EntityProperty)
            {
                case "inputs":
                    return entity.BaseProductionInputs.ContainsKey(entitySubProperty) && !entity.BaseProductionInputs[entitySubProperty].Equals(0);
                case "outputs":
                    return entity.BaseProductionOutputs.ContainsKey(entitySubProperty) && !entity.BaseProductionOutputs[entitySubProperty].Equals(0);
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}