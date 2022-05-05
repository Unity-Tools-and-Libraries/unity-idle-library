using BreakInfinity;
using System;

namespace io.github.thisisnozaku.idle.framework.Modifiers
{
    public abstract class ValueModifier
    {
        public readonly string Id;
        public readonly string Description;
        public readonly int priority;
        public ValueModifier(string id, string description, int priority = 0)
        {
            this.Id = id;
            this.Description = description;
            this.priority = priority;
        }

        public abstract object Apply(object input);
    }
}
