using System;
/*
 * Use this attribute on a method which will recieve event notifications to document which event is handles.
 */
namespace io.github.thisisnozaku.idle.framework.Engine
{
    public class HandledEventAttribute : Attribute
    {
        public readonly Type EventType;

        public HandledEventAttribute(Type eventType)
        {
            EventType = eventType;
        }
    }
}