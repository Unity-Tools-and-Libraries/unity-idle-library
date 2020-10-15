using BreakInfinity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework.Configuration
{
    /*
     * Configure how the value of a ValueReference is calculated.
     */
    public class ValueReferenceDefinition
    {
        private object startingValue;
        private Func<IdleEngine, ValueReference, float, object, object> updater;
        internal ValueReferenceDefinition(object startingValue, Func<IdleEngine, ValueReference, float, object, object> updater)
        {
            this.startingValue = startingValue;
            this.updater = updater;
        }
        public ValueReference CreateValueReference(IdleEngine engine)
        {
            return CreateValueReference(engine, null);
        }

        public ValueReference CreateValueReference(IdleEngine engine, ValueReference containingReference)
        {
            if(engine == null)
            {
                throw new ArgumentException("engine cannot be null");
            }
            if (HasChildValues) // Recursively create child references
            {
                var mapValues = new ParentNotifyingMap();
                var mapReference = new ValueReference(containingReference, mapValues);
                foreach (var entry in StartingValue as Dictionary<string, ValueReferenceDefinition>)
                {
                    mapValues[entry.Key] = entry.Value.CreateValueReference(engine, mapReference);
                }
                engine.RegisterReference(mapReference);
                return mapReference;
            }
            var newReference = new ValueReference(containingReference, startingValue, updater); ;
            engine.RegisterReference(newReference);
            return newReference;
        }

        public bool HasChildValues
        {
            get => startingValue is Dictionary<string, ValueReferenceDefinition>;
        }

        public object StartingValue => startingValue;

        public static implicit operator ValueReferenceDefinition(string value)
        {
            return new ValueReferenceDefinitionBuilder().WithStartingValue(value).Build();
        }

        public static implicit operator ValueReferenceDefinition(BigDouble value)
        {
            return new ValueReferenceDefinitionBuilder().WithStartingValue(value).Build();
        }

        public static implicit operator ValueReferenceDefinition(int value)
        {
            return new ValueReferenceDefinitionBuilder().WithStartingValue(new BigDouble(value)).Build();
        }

        public static implicit operator ValueReferenceDefinition(bool value)
        {
            return new ValueReferenceDefinitionBuilder().WithStartingValue(value).Build();
        }

        public static implicit operator ValueReferenceDefinition(Dictionary<string, ValueReferenceDefinition> value)
        {
            return new ValueReferenceDefinitionBuilder().WithStartingValue(value).Build();
        }
    }
    // TODO: Add conversion from literals
    // TODO: Support for immutable values.
    public class ValueReferenceDefinitionBuilder
    {
        private object startingValue;
        private Func<IdleEngine, ValueReference, float, object, object> updater;

        public static ValueReferenceDefinition NONE => new ValueReferenceDefinitionBuilder().Build();

        public ValueReferenceDefinition Build()
        {
            return new ValueReferenceDefinition(startingValue, updater);
        }

        public ValueReferenceDefinitionBuilder WithStartingValue(string startingValue)
        {
            this.startingValue = startingValue;
            return this;
        }

        public ValueReferenceDefinitionBuilder WithStartingValue(bool startingValue)
        {
            this.startingValue = startingValue;
            return this;
        }

        public ValueReferenceDefinitionBuilder WithStartingValue(IDictionary<string, ValueReferenceDefinition> startingValue)
        {
            this.startingValue = startingValue;
            return this;
        }

        public ValueReferenceDefinitionBuilder WithStartingValue(BigDouble startingValue)
        {
            this.startingValue = startingValue;
            return this;
        }
        /*
         * Define a functions which calculates the value of this reference.
         */
        public ValueReferenceDefinitionBuilder WithUpdater(Func<IdleEngine, ValueReference, float, object, object> updater)
        {
            this.updater = updater;
            return this;
        }
    }
}