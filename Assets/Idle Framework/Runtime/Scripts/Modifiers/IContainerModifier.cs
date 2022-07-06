using io.github.thisisnozaku.idle.framework.Engine;
using System;
using System.Collections.Generic;

namespace io.github.thisisnozaku.idle.framework.Modifiers
{
    public interface IContainerModifier
    {
        string Id { get; }
        string Source { get; }
        Dictionary<string, object> Properties { get; }
        bool IsCached { get; }
        int Order { get; }
        object Apply(IdleEngine engine, ValueContainer container, object input);
        void OnAdd(IdleEngine engine, ValueContainer container);
        void OnRemove(IdleEngine engine, ValueContainer container);
        void Trigger(IdleEngine engine, string eventName, ScriptingContext context = null);
        bool CanApply(IdleEngine engine, ValueContainer container, object intermediateValue);
    }

    public class ContainerModifierComparer : IComparer<IContainerModifier>
    {
        public int Compare(IContainerModifier x, IContainerModifier y)
        {
            int priorityDiff = x.Order - y.Order;
            if (priorityDiff != 0)
            {
                return priorityDiff;
            }
            return x.GetHashCode() - y.GetHashCode();
        }
    }
}