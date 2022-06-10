using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Engine;
using io.github.thisisnozaku.idle.framework.Events;
using System;
using System.Collections.Generic;
using UnityEngine;
using static io.github.thisisnozaku.idle.framework.ValueContainer.Context;

namespace io.github.thisisnozaku.idle.framework.Modifiers.Values
{
    /*
     * A ContainerModifier which only modifies the calculated value of the target container.
     */
    public abstract class ValueModifier : ContainerModifier
    {
        public event Action CacheChanged;
        protected object cachedValue;
        private string expression;
        public string[] Dependencies { get; }
        protected ValueModifier(string id, string source, string expression, string[] dependencies, bool staticValue = false, ContextGenerator contextGenerator = null, int priority = 0) : base(id, source, contextGenerator: contextGenerator, priority: priority)
        {
            this.expression = expression;
            this.IsCached = dependencies == null || dependencies.Length == 0;
            this.Dependencies = dependencies;
        }

        public override abstract object Apply(IdleEngine engine, ValueContainer container, object input);

        public override void OnUpdate(IdleEngine engine, ValueContainer container)
        {
            
        }

        protected T EvaluateCalculationExpression<T>(IdleEngine engine, ValueContainer container)
        {
            if (cachedValue != null)
            {
                return (T)cachedValue;
            }
            var evaluatedExpression = engine.EvaluateExpression(expression, this.GenerateContext(engine, container));
            if (cachedValue == null)
            {
                cachedValue = evaluatedExpression;
            }
            return (T)ValueContainer.NormalizeValue(evaluatedExpression);
        }

        public override bool SupportsType(Type type)
        {
            return typeof(BigDouble) == type;
        }

        public override void OnAdd(IdleEngine engine, ValueContainer container)
        {
            base.OnAdd(engine, container);
            engine.Log(UnityEngine.LogType.Log, "Calling on-add of modifier " + Id, "engine.internal.modifier");
            engine.RegisterMethod(Id + "CacheClear", CacheClear);
            if (Dependencies != null)
            {
                foreach (var dependency in Dependencies)
                {
                    engine.Log(UnityEngine.LogType.Log, String.Format("Modifier {0} subscribing to dependency '{1}'", Id, dependency), "engine.internal.modifier");
                    var targetContainer = engine.EvaluateExpression(dependency, GenerateContext(engine, container));
                    var targetPath = targetContainer != null ? (targetContainer as ValueContainer).Path : dependency;
                    engine.GetOrCreateContainerByPath(targetPath).Subscribe(Id + "modifier dependency", ValueChangedEvent.EventName, Id + "CacheClear");
                }
            } else
            {
                engine.Log(UnityEngine.LogType.Log, "Modifier " + Id + " has no dependencies", "engine.internal.modifier");
            }
        }

        private object CacheClear(IdleEngine engine, ValueContainer container, params object[] args)
        {
            engine.Log(LogType.Log, String.Format("Clearing cached value of modifier {0}", Id), "engine.internal.modifier");
            cachedValue = null;
            if(CacheChanged != null)
            {
                CacheChanged.Invoke();
            }
            return null;
        }

        public override void OnRemove(IdleEngine engine, ValueContainer container)
        {
            base.OnRemove(engine, container);
        }

        public static class DefaultPriorities
        {
            public const int MULTIPLICATION = 1000;
            public const int ADDITION = 2000;
        }
    }
}