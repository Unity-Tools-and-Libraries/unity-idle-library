using BreakInfinity;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using io.github.thisisnozaku.idle.framework.Modifiers;

namespace io.github.thisisnozaku.idle.framework.Configuration
{
    /*
     * Configure how the value of a ValueReference is calculated.
     */
    public class ValueContainerDefinition
    {
        private object startingValue;
        private Func<IdleEngine, float, object, ValueContainer, List<ValueModifier>, object> updater;
        private Action<IdleEngine, float, object> postUpdateHook;
        private List<ValueModifier> startingModifiers;

        internal ValueContainerDefinition(object startingValue,
            Func<IdleEngine, float, object, ValueContainer, List<ValueModifier>, object> updater,
            Action<IdleEngine, float, object> postUpdateHook,
            List<ValueModifier> startingModifiers)
        {
            this.startingValue = startingValue;
            this.updater = updater;
            this.postUpdateHook = postUpdateHook;
            this.startingModifiers = startingModifiers;
        }

        public ValueContainer CreateValueReference(IdleEngine engine)
        {
            return CreateValueReference(engine, null);
        }

        public ValueContainer CreateValueReference(IdleEngine engine, ValueContainer parent)
        {
            if (engine == null)
            {
                throw new ArgumentException("engine cannot be null");
            }
            if (HasChildValues) // Recursively create child references
            {
                Debug.Log("Generating child map with " + (startingValue as IDictionary<string, ValueContainerDefinition>).Count + " entries.");
                var mapValues = new Dictionary<string, ValueContainer>();
                var mapReference = new ValueContainer(parent, (string)null, postUpdateHook);
                foreach (var entry in StartingValue as IDictionary<string, ValueContainerDefinition>)
                {
                    mapValues[entry.Key] = entry.Value.CreateValueReference(engine, mapReference);
                }
                mapReference.Set(mapValues);

                Debug.Log("Final map has " + mapValues.Count + " entries.");
                engine.RegisterReference(mapReference);
                return mapReference;
            }
            var newReference = new ValueContainer(parent, startingValue, updater, postUpdateHook, startingModifiers);
            engine.RegisterReference(newReference);
            return newReference;
        }

        public bool HasChildValues
        {
            get => startingValue is IDictionary<string, ValueContainerDefinition>;
        }

        public object StartingValue => startingValue;

        public static implicit operator ValueContainerDefinition(string value)
        {
            return new ValueContainerDefinitionBuilder().WithStartingValue(value).Build();
        }

        public static implicit operator ValueContainerDefinition(BigDouble value)
        {
            return new ValueContainerDefinitionBuilder().WithStartingValue(value).Build();
        }

        public static implicit operator ValueContainerDefinition(int value)
        {
            return new ValueContainerDefinitionBuilder().WithStartingValue(new BigDouble(value)).Build();
        }

        public static implicit operator ValueContainerDefinition(bool value)
        {
            return new ValueContainerDefinitionBuilder().WithStartingValue(value).Build();
        }

        public static implicit operator ValueContainerDefinition(Dictionary<string, ValueContainerDefinition> value)
        {
            return new ValueContainerDefinitionBuilder().WithStartingValue(value).Build();
        }
    }
    /*
     * Builder to generate a value container.
     * TODO: Implement an automatic conversion between this and a ValueContainerDefinition, which automatically builds this.
     */
    public class ValueContainerDefinitionBuilder
    {
        private object startingValue;
        private Func<IdleEngine, float, object, ValueContainer, List<ValueModifier>, object> updater;
        private Action<IdleEngine, float, object> postUpdateHook;
        private List<ValueModifier> startingModifiers = new List<ValueModifier>();
        /*
         * Constant for an empty devinition.
         */
        public static ValueContainerDefinition EMPTY => new ValueContainerDefinitionBuilder().Build();
        /*
         * Generate the value definition.
         */
        public ValueContainerDefinition Build()
        {
            return new ValueContainerDefinition(startingValue, updater, postUpdateHook, startingModifiers);
        }
        /*
         * Specify an action which is executed after this value container is updated each tick.
         */
        public ValueContainerDefinitionBuilder WithPostUpdateHook(Action<IdleEngine, float, object> hook)
        {
            this.postUpdateHook = hook;
            return this;
        }
        /*
         * Specify the string value this container initially holds.
         */
        public ValueContainerDefinitionBuilder WithStartingValue(string startingValue)
        {
            this.startingValue = startingValue;
            return this;
        }
        /*
         * Specify the bool value this container initially holds.
         */
        public ValueContainerDefinitionBuilder WithStartingValue(bool startingValue)
        {
            this.startingValue = startingValue;
            return this;
        }
        /*
         * Specify the dicionary value this container initially holds.
         */
        public ValueContainerDefinitionBuilder WithStartingValue(IDictionary<string, ValueContainerDefinition> startingValue)
        {
            this.startingValue = startingValue;
            return this;
        }
        /*
         * Specify the number value this container initially holds.
         */
        public ValueContainerDefinitionBuilder WithStartingValue(BigDouble startingValue)
        {
            this.startingValue = startingValue;
            return this;
        }
        /*
         * Define a functions which calculates the value of this reference.
         * 
         * The function is called each tick with an instance of the engine, the time passed since the last tick, and the previous value.
         * 
         * If an updater function is specified, the resulting Value Container will throw an error if you attempt to set its value; only the updater can modify the value.
         */
        public ValueContainerDefinitionBuilder WithUpdater(Func<IdleEngine, float, object, ValueContainer, List<ValueModifier>, object> updater)
        {
            this.updater = updater;
            return this;
        }

        /*
         * Specify a modifier which will apply to the resulting value container by default.
         */
        public ValueContainerDefinitionBuilder WithModifier(ValueModifier modifier)
        {
            if (startingModifiers.Any(m => m.Id == modifier.Id))
            {
                throw new InvalidOperationException(string.Format("A modifier with id {0} is already specified, ids must be unique.", modifier.Id));
            }
            startingModifiers.Add(modifier);
            return this;
        }
    }
}