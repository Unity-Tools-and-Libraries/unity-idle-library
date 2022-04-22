using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Configuration;
using io.github.thisisnozaku.idle.framework.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using UnityEngine;
namespace io.github.thisisnozaku.idle.framework
{
    public class IdleEngine : EventSource
    {
        private Dictionary<string, ValueContainer> references = new Dictionary<string, ValueContainer>();
        private Dictionary<string, ValueContainer> globalProperties = new Dictionary<string, ValueContainer>();
        private Dictionary<string, List<Action<object>>> listeners = new Dictionary<string, List<Action<object>>>();
        public Dictionary<string, List<Action<object>>> EventListeners => listeners;


        /*
         * * Attempts to create a new instance
         */
        public void GetOrCreateEntity(string entityType)
        {
            throw new UndefinedEntityException(entityType);
        }


        public IdleEngine(EngineConfiguration configuration, GameObject gameObject)
        {
            if (configuration?.GlobalProperties != null)
            {
                globalProperties = recursivelyCreateValueReferences(configuration.GlobalProperties);
            }
        }

        private Dictionary<string, ValueContainer> recursivelyCreateValueReferences(IDictionary<string, ValueContainerDefinition> propertyDefinitions)
        {
            var properties = new Dictionary<string, ValueContainer>();
            foreach (var globalPropertyEntry in propertyDefinitions)
            {
                properties[globalPropertyEntry.Key] = globalPropertyEntry.Value.CreateValueReference(this);
            }
            return properties;
        }


        public ValueContainer GetGlobalProperty(string property)
        {
            if (!globalProperties.ContainsKey(property))
            {
                throw new UndefinedPropertyException(property, "global properties");
            }
            return globalProperties[property];
        }

        public void Update(float deltaTime)
        {
            ClearUpdateFlags();
            var toIterate = new List<ValueContainer>(references.Values);
            foreach (var reference in toIterate)
            {
                reference.Update(this, deltaTime);
            }
            AssertAllEntitiesUpdated();
        }

        private void AssertAllEntitiesUpdated()
        {
            int unupdatedRefsCount = references.Values.Where(x => !x.UpdatedThisTick).Count();
            if (unupdatedRefsCount > 0)
            {
                Debug.LogError(string.Format("{0} ref failed to update this tick.", unupdatedRefsCount));
            }
        }

        private void ClearUpdateFlags()
        {
            foreach (var reference in references.ToList())
            {
                reference.Value.ClearUpdatedFlag();
            }
        }

        public void RegisterReference(ValueContainer newReference)
        {
            if (newReference.Id == null)
            {
                newReference.Id = (references.Count() + 1).ToString();
                references.Add(newReference.Id, newReference);
            }
            if(newReference.ValueAsMap() != null)
            {
                foreach(var child in newReference.ValueAsMap().Values)
                {
                    RegisterReference(child);
                }
            }
        }

        internal ValueContainer GetReferenceById(string internalId)
        {
            return references[internalId];
        }

        public void Subscribe(string eventName, Action<object> listener)
        {
            List<Action<object>> eventListeners;
            if (!listeners.TryGetValue(eventName, out eventListeners))
            {
                eventListeners = new List<Action<object>>();
                listeners[eventName] = eventListeners;
            }
            eventListeners.Add(listener);
        }

        public static bool CoerceToBool(object value)
        {
            if (value is bool)
            {
                return (bool)value;
            }
            else if (value is IDictionary<string, ValueContainer>)
            {
                return true;
            }
            else
            {
                return (BigDouble)value != BigDouble.Zero;
            }
        }

        public static BigDouble CoerceToNumber(object value)
        {
            if (value is bool)
            {
                return (bool)value ? BigDouble.One : BigDouble.Zero;
            }
            else if (value is IDictionary<string, ValueContainer>)
            {
                return BigDouble.Zero;
            }
            else
            {
                return (BigDouble)value;
            }
        }
        public static string CoerceToString(object value)
        {
            if (value is string)
            {
                return (string)value;
            }
            return value.ToString();
        }
        public static IDictionary<string, ValueContainer> CoerceToMap(object value)
        {
            if (value is BigDouble || value is string || value is bool)
            {
                return null;
            }
            if (!(value is IDictionary<string, ValueContainer>))
            {
                throw new InvalidOperationException(string.Format("Failed to coerce a value of type {0} to IDictionary<string, ValueReference>.", value.GetType()));
            }
            return value as IDictionary<string, ValueContainer>;
        }
    }
}