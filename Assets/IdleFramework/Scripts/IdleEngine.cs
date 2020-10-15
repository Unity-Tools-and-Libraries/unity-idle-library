using IdleFramework.Configuration;
using IdleFramework.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using UnityEditor.Graphs;
using UnityEngine;
namespace IdleFramework
{
    public class IdleEngine
    {
        private Dictionary<string, ValueReference> references = new Dictionary<string, ValueReference>();
        private Dictionary<string, ValueReference> globalProperties = new Dictionary<string, ValueReference>();
        /*
         * Attempts to create a new instance
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

        private Dictionary<string, ValueReference> recursivelyCreateValueReferences(IDictionary<string, ValueReferenceDefinition> propertyDefinitions)
        {
            var properties = new Dictionary<string, ValueReference>();
            foreach (var globalPropertyEntry in propertyDefinitions)
            {
                properties[globalPropertyEntry.Key] = globalPropertyEntry.Value.CreateValueReference(this);
            }
            return properties;
        }
        

        public ValueReference GetGlobalProperty(string property)
        {
            if(!globalProperties.ContainsKey(property))
            {
                throw new UndefinedPropertyException(property, "global properties");
            }
            return globalProperties[property];
        }

        public void Update(float deltaTime)
        {
            ClearUpdateFlags();
            foreach(var reference in references)
            {
                reference.Value.Update(this, deltaTime);
            }
            AssertAllEntitiesUpdated();
        }

        private void AssertAllEntitiesUpdated()
        {
            int unupdatedRefsCount = references.Values.Where(x => !x.UpdatedThisTick).Count();
            if(unupdatedRefsCount > 0)
            {
                Debug.LogError(string.Format("{0} ref failed to update this tick.", unupdatedRefsCount));
            }
        }

        private void ClearUpdateFlags()
        {
            foreach(var reference in references.ToList())
            {
                reference.Value.ClearUpdatedFlag();
            }
        }

        internal void RegisterReference(ValueReference newReference)
        {
            references.Add(newReference.Id, newReference);
        }
    }
}