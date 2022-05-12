using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Events;
using io.github.thisisnozaku.idle.framework.Modifiers;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace io.github.thisisnozaku.idle.framework
{
    public class IdleEngine : EventSource
    {
        private IDictionary<string, ValueContainer> references = new Dictionary<string, ValueContainer>();
        public readonly IDictionary<string, ValueContainer> globalProperties = new Dictionary<string, ValueContainer>();
        private Dictionary<string, List<Action<object>>> listeners = new Dictionary<string, List<Action<object>>>();
        public Dictionary<string, List<Action<object>>> EventListeners => listeners;

        public IdleEngine(GameObject gameObject)
        {

        }

        public ValueContainer GetGlobalProperty(string property)
        {
            if (!globalProperties.ContainsKey(property))
            {
                globalProperties[property] = CreateValueContainer();
            }
            return globalProperties[property];
        }

        public void SetGlobalProperty(string property, bool value)
        {
            GetGlobalProperty(property).Set(value);
        }

        public void SetGlobalProperty(string property, string value = null)
        {
            GetGlobalProperty(property).Set(value);
        }

        public void SetGlobalProperty(string property, IDictionary<string, ValueContainer> value)
        {
            GetGlobalProperty(property).Set(value);
        }

        public void SetGlobalProperty(string property, BigDouble value)
        {
            GetGlobalProperty(property).Set(value);
        }

        public void SetGlobalProperty(string property, Func<IdleEngine, ValueContainer, object[], object> value)
        {
            GetGlobalProperty(property).Set(value);
        }

        public void Update(float deltaTime)
        {
            foreach (var reference in references.Values)
            {
                reference.ClearUpdatedFlag();
            }
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

        private ValueContainer RegisterReference(ValueContainer newReference)
        {
            if (newReference.Id == null)
            {
                newReference.Id = (references.Count() + 1).ToString();
                references.Add(newReference.Id, newReference);
            }
            if (newReference.ValueAsMap() != null)
            {
                foreach (var child in newReference.ValueAsMap().Values)
                {
                    RegisterReference(child);
                }
            }

            return newReference;
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

        public ValueContainer CreateValueContainer(string value = null, List<ValueModifier> modifiers = null, ValueContainer.UpdatingMethod updater = null)
        {
            var vc = new ValueContainer(this, value, modifiers, updater);
            RegisterReference(vc);
            return vc;
        }

        public ValueContainer CreateValueContainer(BigDouble value, List<ValueModifier> modifiers = null, ValueContainer.UpdatingMethod updater = null)
        {
            var vc = new ValueContainer(this, value, modifiers, updater);
            RegisterReference(vc);
            return vc;
        }

        public ValueContainer CreateValueContainer(bool value, List<ValueModifier> modifiers = null, ValueContainer.UpdatingMethod updater = null)
        {
            var vc = new ValueContainer(this, value, modifiers, updater);
            RegisterReference(vc);
            return vc;
        }

        public ValueContainer CreateValueContainer(IDictionary<string, ValueContainer> value, List<ValueModifier> modifiers = null, ValueContainer.UpdatingMethod updater = null)
        {
            var vc = new ValueContainer(this, value, modifiers, updater);
            RegisterReference(vc);
            return vc;
        }

        public ValueContainer CreateValueContainer(Func<IdleEngine, ValueContainer, object[], object> value)
        {
            var vc = new ValueContainer(this, value);
            return RegisterReference(vc);
        }

        public void Notify(string eventName, object argument)
        {
            List<Action<object>> listeners;
            if(this.listeners.TryGetValue(eventName, out listeners))
            {
                foreach(var listener in listeners)
                {
                    listener.Invoke(argument);
                }
            }
        }
    }
}