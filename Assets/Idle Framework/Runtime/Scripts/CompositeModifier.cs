using BreakInfinity;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using io.github.thisisnozaku.idle.framework.Modifiers.Values;

namespace io.github.thisisnozaku.idle.framework.Modifiers
{
    /*
     * A CompositeModifier is 
     */
    public abstract class CompositeModifier : ContainerModifier
    {
        public Dictionary<string, string> Modifications { get; }
        public Dictionary<string, List<string>> Events { get; }

        protected CompositeModifier(string id, string description, Dictionary<string, string> Modifications, Dictionary<string, List<string>> Events = null, int priority = 0) : base(id, description, priority)
        {
            this.Events = Events;
            this.Modifications = Modifications;
        }

        public override void OnAdd(IdleEngine engine, ValueContainer target)
        {
            if (Modifications != null)
            {
                foreach (var effect in Modifications)
                {
                    try
                    {
                        ValueContainer effectTarget = engine.GetOrCreateContainerByPath(string.Join(".", target.Path, effect.Key));
                        ContainerModifier modifier = GenerateValueModifier(target.Engine, Id, effectTarget, effect.Value);
                        effectTarget.AddModifier(modifier);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError("An error occured while applying modifier " + Id + ": " + ex.ToString());
                    }
                }
            }
        }

        public override void OnRemoval(IdleEngine engine, ValueContainer target)
        {
            foreach (var effect in Modifications)
            {
                try
                {
                    ValueContainer effectTarget = target.GetProperty(effect.Key);
                    if (effectTarget == null)
                    {
                        throw new InvalidOperationException("No contained at path " + String.Join(".", target.Path, effect.Key) + " found.");
                    }
                    var toRemove = effectTarget.GetModifiers().First(m => m.Id == Id && m.Source == this.Id);
                    effectTarget.RemoveModifier(toRemove);
                }
                catch (Exception ex)
                {
                    Debug.LogError("An error occured while unapplying modifier " + Id + ": " + ex.ToString());
                }
            }
        }

        public override void Trigger(IdleEngine engine, string eventName) {
            if (Events != null)
            {
                List<string> actions;
                if (Events.TryGetValue(eventName, out actions))
                {
                    foreach (var action in actions)
                    {
                        engine.EvaluateExpression(action);
                    }
                }
            }
        }

        private ContainerModifier GenerateValueModifier(IdleEngine engine, string Id, ValueContainer target, string expression)
        {
            // Get operator
            char modifierOperator = expression[0];
            string valueExpression = expression.Substring(1);
            if (!ContainerModifier.ALL_OPERATORS.Contains(modifierOperator))
            {
                throw new ArgumentException("Unknown operator " + modifierOperator);
            }
            switch(modifierOperator)
            {
                case ContainerModifier.ADD_OPERATOR:
                    return new AdditiveValueModifier(Id, this.Id, valueExpression);
                case ContainerModifier.SUBTRACT_OPERATOR:
                    return new SubtractiveValueModifier(Id, this.Id, valueExpression);
                case ContainerModifier.MULTIPLY_OPERATOR:
                    return new MultiplicativeValueModifier(Id, this.Id, valueExpression);
                case ContainerModifier.DIVIDE_OPERATOR:
                    return new DivisionValueModifier(Id, this.Id, valueExpression);
                case ContainerModifier.ASSIGN_OPERATOR:
                    return new SetValueModifier(Id, this.Id, valueExpression);
            }
            throw new InvalidOperationException();
        }

        
    }
}