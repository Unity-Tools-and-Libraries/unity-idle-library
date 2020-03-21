using System;

namespace IdleFramework
{
    public interface EngineHookDefinitionProperties<in I, out O>
    {
        EngineHookAction Action { get; }
        string Actor { get; }
        string Subject { get; }
        Func<I, O> Function { get; }
    }
}