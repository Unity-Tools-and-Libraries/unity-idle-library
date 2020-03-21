namespace IdleFramework
{
    public interface HookExecutionContext<T>
    {
        T Payload { get; }
        string Actor { get; }
    }
}