using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Engine;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace io.github.thisisnozaku.idle.framework.Modifiers
{
    /*
     * Base class for an item which modifies the value output from a container, and/or modifies child values.
     */
    public abstract class ContainerModifier : IContainerModifier, ScriptingContext
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
        protected ScriptingContext scriptingContext;

        public ContainerModifier(string id, string source, ScriptingContext context = null, int priority = 0)
        {
            this.Id = id;
            this.Source = source;
            this.Order = priority;
            scriptingContext = context;
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
        public virtual void Trigger(IdleEngine engine, string eventName, ScriptingContext context = null) { }



        public abstract bool CanApply(IdleEngine engine, ValueContainer container, object intermediateValue);
        public override string ToString()
        {
            return String.Format("{0} {1}", GetType().Name, Id);
        }

        public override bool Equals(object obj)
        {
            return obj is ContainerModifier modifier &&
                   Id == modifier.Id &&
                   Source == modifier.Source &&
                   Order == modifier.Order &&
                   EqualityComparer<Dictionary<string, object>>.Default.Equals(Properties, modifier.Properties) &&
                   IsCached == modifier.IsCached &&
                   EqualityComparer<ScriptingContext>.Default.Equals(scriptingContext, modifier.scriptingContext);
        }

        public override int GetHashCode()
        {
            int hashCode = 146449135;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Id);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Source);
            hashCode = hashCode * -1521134295 + Order.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<Dictionary<string, object>>.Default.GetHashCode(Properties);
            hashCode = hashCode * -1521134295 + IsCached.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<ScriptingContext>.Default.GetHashCode(scriptingContext);
            return hashCode;
        }

        public Dictionary<string, object> GetScriptingContext(string contextType = null)
        {
            return new Dictionary<string, object>()
            {
                { "this", this }
            };
        }
    }
}
