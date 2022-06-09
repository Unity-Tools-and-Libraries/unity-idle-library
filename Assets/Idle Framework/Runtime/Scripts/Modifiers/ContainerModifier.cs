using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Engine;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using static io.github.thisisnozaku.idle.framework.ValueContainer.Context;

namespace io.github.thisisnozaku.idle.framework.Modifiers
{
    /*
     * Base class for an item which modifies the value output from a container, and/or modifies child values.
     */
    public abstract class ContainerModifier : IContainerModifier
    {
        public static readonly List<char> ALL_OPERATORS = new List<char>() { ADD_OPERATOR, SUBTRACT_OPERATOR, MULTIPLY_OPERATOR, DIVIDE_OPERATOR, ASSIGN_OPERATOR };
        public const char ADD_OPERATOR = '+';
        public const char SUBTRACT_OPERATOR = '-';
        public const char MULTIPLY_OPERATOR = '*';
        public const char DIVIDE_OPERATOR = '/';
        public const char ASSIGN_OPERATOR = '=';

        [JsonProperty("id")]
        public string Id { get; private set; }
        public string Source { get; }
        public int Order { get; }
        [JsonProperty("properties")]
        public Dictionary<string, object> Properties { get; private set; }
        public bool IsCached { get; protected set; }
        private ContextGenerator contextGenerator;

        public ContainerModifier(string id, string source, ContextGenerator contextGenerator = null, int priority = 0)
        {
            this.Id = id;
            this.Source = source;
            this.Order = priority;
            this.contextGenerator = contextGenerator != null ? contextGenerator : ValueContainer.Context.DefaultGenerator;
            Properties = new Dictionary<string, object>();
        }
        /*
         * Method called when a modifier is used to calculate the value held by the container.
         */
        public virtual object Apply(IdleEngine engine, ValueContainer container, object input) {
            return input;
        }

        /*
         * Method called when a modifier is added to a ValueContainer. Override when a modifier has effects on things other than that contained by the target container.
         */
        public virtual void OnAdd(IdleEngine engine, ValueContainer container)
        {

        }

        /*
         * Method called when a modifier is removed from a ValueContainer. Override this when overriding OnAdd to perform cleanup.
         */
        public virtual void OnRemove(IdleEngine engine, ValueContainer container)
        {

        }

        public virtual void OnUpdate(IdleEngine engine, ValueContainer container) { }
        public virtual void Trigger(IdleEngine engine, string eventName) { }

        public virtual IDictionary<string, object> GenerateContext(IdleEngine engine, ValueContainer container)
        {
            return contextGenerator(engine, container);
        }

        public override bool Equals(object obj)
        {
            return obj is ContainerModifier modifier &&
                   Id == modifier.Id &&
                   Source == modifier.Source &&
                   Order == modifier.Order &&
                   EqualityComparer<Dictionary<string, object>>.Default.Equals(Properties, modifier.Properties) &&
                   IsCached == modifier.IsCached &&
                   EqualityComparer<ContextGenerator>.Default.Equals(contextGenerator, modifier.contextGenerator);
        }

        public override int GetHashCode()
        {
            int hashCode = 1199913225;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Id);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Source);
            hashCode = hashCode * -1521134295 + Order.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<Dictionary<string, object>>.Default.GetHashCode(Properties);
            hashCode = hashCode * -1521134295 + IsCached.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<ContextGenerator>.Default.GetHashCode(contextGenerator);
            return hashCode;
        }

        public abstract bool SupportsType(Type type);
    }
}
