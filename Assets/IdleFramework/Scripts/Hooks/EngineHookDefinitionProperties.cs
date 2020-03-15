namespace IdleFramework
{
    public interface EngineHookDefinitionProperties
    {
        EngineHookAction Action { get; }
        string Actor { get; }
        string Subject { get; }
    }
}