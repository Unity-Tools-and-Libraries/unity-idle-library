namespace IdleFramework
{
    internal interface HookExecutionContext<T>
    {
        T Payload { get; }
        string Actor { get; }
    }
}