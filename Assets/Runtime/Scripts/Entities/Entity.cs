using io.github.thisisnozaku.idle.framework.Engine.Persistence;
using io.github.thisisnozaku.idle.framework.Events;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using UnityEngine;
using MoonSharp.Interpreter;
using io.github.thisisnozaku.scripting.context;

namespace io.github.thisisnozaku.idle.framework.Engine
{
    /*
     * Base class for custom types.
     */
    public abstract class Entity : IEventSource, IEngineAware, ITraversableType, IUpdateable
    {
        [JsonIgnore]
        public IdleEngine Engine { get; private set; }
        public Dictionary<string, bool> Flags { get; private set; }
        [JsonProperty]
        private EventListeners eventListeners;
        [JsonProperty]
        private Dictionary<string, string> propertyCalculationScripts = new Dictionary<string, string>();
        [JsonProperty]
        private List<long> appliedModifiers = new List<long>();
        private List<MemberInfo> traversableFields;

        public long Id { get; }
        /*
         * Return the list of the ids of all modifiers applied to this entity.
         * 
         * Override this method and change it to return null to signal that this entity will not accept modifiers.
         */
        public List<long> GetModifiers() => appliedModifiers;

        public Entity(IdleEngine engine, long id)
        {
            this.Engine = engine;
            this.Id = id;
            
            Flags = new Dictionary<string, bool>();
            this.eventListeners = new EventListeners(engine);
            this.ExtraProperties = new Dictionary<string, object>();
            if (engine != null)
            {
                engine.RegisterEntity(this);
            }
            traversableFields = new List<MemberInfo>();
            FieldInfo[] fields = GetType().GetFields();
            foreach(var field in fields)
            {
                if(field.GetCustomAttribute<TraversableFieldOrProperty>() != null)
                {
                    traversableFields.Add(field);
                }
            }
            PropertyInfo[] properties = GetType().GetProperties();
            foreach (var property in properties)
            {
                if (property.GetCustomAttribute<TraversableFieldOrProperty>() != null)
                {
                    traversableFields.Add(property);
                }
            }
        }

        [OnDeserialized]
        public void OnDeserialization(StreamingContext ctx)
        {
            this.Engine = (IdleEngine)ctx.Context;
            this.Engine.RegisterEntity(this);
        }

        /*
         * Generic method to add a modifier to this entity.
         * 
         * Implement methods which accept domain-specific subclasses of PropertyModifier which forward to this method to handle the heavy lifting.
         */
        public void AddModifier<T>(EntityModifier<T> propertyModifier) where T : Entity
        {
            var modifiers = GetModifiers();
            if (modifiers != null)
            {
                propertyModifier.Apply((T)this);
                modifiers.Add(propertyModifier.Id);
            }
        }

        public void RemoveModifier<T>(EntityModifier<T> propertyModifier) where T : Entity
        {
            var modifiers = GetModifiers();
            if (modifiers != null)
            {
                var removed = modifiers.Remove(propertyModifier.Id);
                if (removed)
                {
                    propertyModifier.Unapply((T)this);
                }
                
            }
        }

        private Dictionary<string, Action> calculatedPropertySetters = new Dictionary<string, Action>();
        public void Update(IdleEngine engine, float deltaTime) { 
            foreach(var child in propertyCalculationScripts)
            {
                if(!calculatedPropertySetters.ContainsKey(child.Key))
                {
                    FieldInfo fieldInfo = GetType().GetField(child.Key);
                    var updateScriptContext = new Dictionary<string, object>()
                    {
                        { "this", this},
                        { "value", null }
                    };
                    if (fieldInfo != null)
                    {
                        calculatedPropertySetters[child.Key] = () =>
                        {
                            object currentValue = fieldInfo.GetValue(this);
                            updateScriptContext["value"] = currentValue;
                            fieldInfo.SetValue(this, Engine.Scripting.EvaluateStringAsScript(child.Value, updateScriptContext).ToObject());
                        };
                    }

                    PropertyInfo propertyInfo = GetType().GetProperty(child.Key);
                    if (propertyInfo != null && !calculatedPropertySetters.ContainsKey(child.Key))
                    {
                        calculatedPropertySetters[child.Key] = () =>
                        {
                            object currentValue = propertyInfo.GetValue(this);
                            updateScriptContext["value"] = currentValue;
                            propertyInfo.SetValue(this, Engine.Scripting.EvaluateStringAsScript(child.Value, updateScriptContext).ToObject());
                        };
                    }
                    if(!calculatedPropertySetters.ContainsKey(child.Key))
                    {
                        throw new InvalidOperationException();
                    }
                    
                }
                calculatedPropertySetters[child.Key]();
            }
            CustomUpdate(engine, deltaTime);
        }

        /*
         * Override this method to implement your own update logic in your custom entities.
         */
        protected virtual void CustomUpdate(IdleEngine engine, float deltaTime) { 

        }

        public void CalculateChild(string childValueSelector, string script)
        {
            propertyCalculationScripts[childValueSelector] = script;
        }


        public virtual void Emit(string eventName, IDictionary<string, object> contextToUse)
        {
            eventListeners.Emit(eventName, contextToUse);
        }

        public virtual void Emit(string eventName, IScriptingContext contextToUse = null)
        {
            eventListeners.Emit(eventName, contextToUse);
        }

        public virtual void Emit(string eventName, Tuple<string, object> contextToUse)
        {
            eventListeners.Emit(eventName, contextToUse);
        }

        public void Watch(string eventName, string subscriber, string handler)
        {
            ((IEventSource)eventListeners).Watch(eventName, subscriber, handler);
        }

        public void Watch(string eventName, string subscriber, DynValue handler)
        {
            eventListeners.Watch(eventName, subscriber, handler);
        }

        public void StopWatching(string eventName, string subscriptionTarget)
        {
            ((IEventSource)eventListeners).StopWatching(eventName, subscriptionTarget);
        }

        public void SetFlag(string flag) => Flags[flag] = true;

        public void ClearFlag(string flag) => Flags.Remove(flag);

        public bool GetFlag(string flag)
        {
            return Flags.ContainsKey(flag) && Flags[flag];
        }

        public IEnumerable GetTraversableFields() => traversableFields.Select(f =>
        {
            if(f is PropertyInfo)
            {
                return (f as PropertyInfo).GetValue(this);
            } else if(f is FieldInfo)
            {
                return (f as FieldInfo).GetValue(this);
            } else
            {
                throw new InvalidOperationException();
            }
        });

        [JsonProperty]
        public Dictionary<string, object> ExtraProperties { get; set; }


    }
}