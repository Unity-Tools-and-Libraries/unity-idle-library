using System.Collections.Generic;

namespace io.github.thisisnozaku.idle.framework.Modifiers
{
    public interface IContainerModifier
    {
        string Id { get; }
        Dictionary<string, object> Properties { get; }

        object Apply(IdleEngine engine, ValueContainer container, object input);
        void OnAdd(IdleEngine engine, ValueContainer container);
        void OnRemoval(IdleEngine engine, ValueContainer container);
        void Trigger(IdleEngine engine, string eventName);
    }
}