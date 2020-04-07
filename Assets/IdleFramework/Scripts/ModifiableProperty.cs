using BreakInfinity;
using System;
using System.Collections.Generic;

namespace IdleFramework
{
    public class ModifiableProperty : Updates, ValueContainer
    {
        private readonly GameEntity parent;
        private readonly string propertyName;
        private ValueContainer baseValue;
        private object calculatedValue;
        private List<ModifierEffect> appliedModifiers = new List<ModifierEffect>();
        private readonly IdleEngine engine;

        public ModifiableProperty(GameEntity parent, string propertyName, ValueContainer baseValue, IdleEngine engine, params ModifierEffect[] initialModifiers)
        {
            this.parent = parent;
            this.propertyName = propertyName;
            this.engine = engine;
            this.baseValue = baseValue;
            this.calculatedValue = this.baseValue.RawValue(engine);
            appliedModifiers.AddRange(initialModifiers);
        }

        private void calculateValue()
        {
            calculatedValue = baseValue.RawValue(engine);
            foreach (var modifierEffect in appliedModifiers)
            {
                calculatedValue = modifierEffect.effect.CalculateEffect(this);
            }
        }

        public IReadOnlyList<ModifierEffect> AppliedModifiers => appliedModifiers.AsReadOnly();

        public GameEntity Parent => parent;

        public void AddModifierEffect(ModifierEffect modifierEffect)
        {
            appliedModifiers.Add(modifierEffect);
        }

        public void RemoveModifierEFfect(ModifierEffect modifierAndEffect)
        {
            appliedModifiers.Remove(modifierAndEffect);
        }

        public override bool Equals(object obj)
        {
            return obj is ModifiableProperty property &&
                   baseValue.Equals(property.baseValue) &&
                   EqualityComparer<List<ModifierEffect>>.Default.Equals(appliedModifiers, property.appliedModifiers);
        }

        public override int GetHashCode()
        {
            var hashCode = 1254980278;
            hashCode = hashCode * -1521134295 + baseValue.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<List<ModifierEffect>>.Default.GetHashCode(appliedModifiers);
            return hashCode;
        }

        public override string ToString()
        {
            return String.Format("Property({0}) with value {2} in {1}", propertyName, parent.EntityKey, calculatedValue);
        }

        public void Update(IdleEngine engine, float deltaTime)
        {
            calculateValue();
        }

        public bool GetAsBoolean(IdleEngine engine)
        {
            if (calculatedValue == null)
            {
                return false;
            }
            if (typeof(BigDouble).Equals(calculatedValue.GetType()))
            {
                return !BigDouble.Zero.Equals(calculatedValue);
            }
            if(typeof(string).Equals(calculatedValue.GetType()))
            {
                bool val;
                bool.TryParse((string)calculatedValue, out val);
                return val;
            }
            if(typeof(bool).Equals(calculatedValue.GetType()))
            {
                return (bool)calculatedValue;
            }
            if(calculatedValue is null)
            {
                return false;
            }
            throw new InvalidOperationException();
        }

        public BigDouble GetAsNumber(IdleEngine engine)
        {
            if(calculatedValue == null)
            {
                return 0;
            }
            if (typeof(BigDouble).Equals(calculatedValue.GetType()))
            {
                return (BigDouble)calculatedValue;
            }
            if (typeof(string).Equals(calculatedValue.GetType()))
            {
                try
                {
                    return BigDouble.Parse((string)calculatedValue);
                } catch(FormatException ex)
                {
                    return 0;
                }
            }
            if (typeof(bool).Equals(calculatedValue.GetType()))
            {
                return (bool)calculatedValue ? 1 : 0;
            }
            if(calculatedValue is null)
            {
                return 0;
            }
            throw new InvalidOperationException();
        }

        public string GetAsString(IdleEngine engine)
        {
            if (calculatedValue == null)
            {
                return "";
            }
            return calculatedValue.ToString();
        }

        public object RawValue(IdleEngine engine)
        {
            return calculatedValue;
        }

        public void SetBaseValue(ValueContainer value)
        {
            this.baseValue = value;
            calculateValue();
        }

        public PropertyContainer GetAsContainer(IdleEngine engine)
        {
            throw new NotImplementedException();
        }
    }
}