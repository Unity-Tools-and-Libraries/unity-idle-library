using BreakInfinity;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using io.github.thisisnozaku.idle.framework.Modifiers.Values;
using io.github.thisisnozaku.idle.framework.Engine;

namespace io.github.thisisnozaku.idle.framework.Modifiers
{
    /*
     * A CompositeModifier is 
     */
    public abstract class CompositeModifier : ContainerModifier
    {
        public IDictionary<string, string> Modifications { get; }
        public IDictionary<string, List<string>> Events { get; }

        protected CompositeModifier(string id, string description, IDictionary<string, string> Modifications, IDictionary<string, List<string>> Events = null, int priority = 0) : base(id, description, priority: priority)
        {
            this.Events = Events;
            this.Modifications = Modifications;
        }

        public override void OnAdd(IdleEngine engine, ValueContainer target)
        {
            if (Modifications != null)
            {
                var context = new Dictionary<string, object>()
                    {
                        { "target", target }
                    };
                foreach (var effect in Modifications)
                {

                    try
                    {
                        ValueContainer effectTarget = (engine.EvaluateExpression(string.Format("return getOrCreate('{0}')", effect.Key), context) as ValueContainer);
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

        public override void OnRemove(IdleEngine engine, ValueContainer target)
        {
            var context = new Dictionary<string, object>()
                    {
                        { "target", target }
                    };
            foreach (var effect in Modifications)
            {
                try
                {
                    ValueContainer effectTarget = (engine.EvaluateExpression("return " + effect.Key, context) as ValueContainer);
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

        public override void Trigger(IdleEngine engine, string eventName, IDictionary<string, object> Context = null)
        {
            if (Events != null)
            {
                List<string> actions;
                if (Events.TryGetValue(eventName, out actions))
                {
                    foreach (var action in actions)
                    {
                        engine.EvaluateExpression(action, Context);
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
            switch (modifierOperator)
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