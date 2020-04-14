/**
 * Describes a matcher used to determine when to trigger a hook.
 */ 
public interface EngineEventSelector
{
    string EventType { get; }
}