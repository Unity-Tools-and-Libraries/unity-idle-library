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

        public override void ApplyEffect(IdleEngine engine, ModifierDefinition parentModifier)
        {
            GameEntity entity = null;
            if(engine.AllEntities.TryGetValue(EntityKey, out entity))
            {
                ApplyEffectToEntity(entity, engine, parentModifier);
            }
            
        }

        protected void ApplyEffectToEntity(GameEntity entity, IdleEngine engine, ModifierDefinition parentModifier)
        {
            BigDouble newValue = calculateValue(getBaseValue(entity, engine));
            switch (entityProperty)
            {
                case "inputs":
                    entity.ProductionInputs[entitySubProperty].Value = newValue;
                    entity.ProductionInputs[entitySubProperty].AppliedModifiers.Add(new ModifierAndEffect(parentModifier, this));
                    break;
                case "outputs":
                    entity.ProductionOutputs[entitySubProperty].Value = newValue;
                    entity.ProductionOutputs[entitySubProperty].AppliedModifiers.Add(new ModifierAndEffect(parentModifier, this));
                    break;
                case "requirements":
                    entity.Requirements[entitySubProperty].Value = newValue;
                    entity.Requirements[entitySubProperty].AppliedModifiers.Add(new ModifierAndEffect(parentModifier, this));
                    break;
                case "costs":
                    entity.Costs[entitySubProperty].Value = newValue;
                    entity.Costs[entitySubProperty].AppliedModifiers.Add(new ModifierAndEffect(parentModifier, this));
                    break;
                case "upkeep":
                    entity.Upkeep[entitySubProperty].Value = newValue;
                    entity.Upkeep[entitySubProperty].AppliedModifiers.Add(new ModifierAndEffect(parentModifier, this));
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
                        return entity.ProductionOutputs[entitySubProperty].Value;
                    } else if (entity.BaseProductionOutputs.ContainsKey(entitySubProperty))
                    {
                        return entity.BaseProductionOutputs[entitySubProperty].Get(engine);
                    }
                    return 0;
                case "inputs":
                    if (entity.ProductionInputs.ContainsKey(entitySubProperty))
                    {
                        return entity.ProductionInputs[entitySubProperty].Value;
                    }
                    else if (entity.BaseProductionInputs.ContainsKey(entitySubProperty))
                    {
                        return entity.BaseProductionInputs[entitySubProperty].Get(engine);
                    }
                    return 0;
                case "upkeep":
                    if (entity.Upkeep.ContainsKey(entitySubProperty))
                    {
                        return entity.Upkeep[entitySubProperty].Value;
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