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
        public string Source;
        public readonly int priority;
        [JsonProperty("properties")]
        public Dictionary<string, object> Properties { get; private set; }
        private ContextGenerator contextGenerator;

        public ContainerModifier(string id, string source, ContextGenerator contextGenerator = null, int priority = 0)
        {
            this.Id = id;
            this.Source = source;
            this.priority = priority;
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
        public virtual void OnRemoval(IdleEngine engine, ValueContainer container)
        {

        }

        public virtual void Trigger(IdleEngine engine, string eventName) { }

        public virtual IDictionary<string, object> GenerateContext(IdleEngine engine, ValueContainer container)
        {
            return contextGenerator(engine, container);
        }
    }
}
