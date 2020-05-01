using BreakInfinity;
using System.Collections.Generic;

namespace IdleFramework.Entities
{
    /*
     * Interface for a stateful entity instance.
     */
    public interface IEntityProperties
    {
        string EntityKey { get; }
        ISet<string> Types { get; }
        Dictionary<string, BigDouble> Costs { get; }
        Dictionary<string, BigDouble> Upkeep { get; }
        Dictionary<string, BigDouble> FixedOutputs { get; }
        Dictionary<string, BigDouble> FixedInputs { get; }
        Dictionary<string, BigDouble> ScaledOutputs { get; }
        Dictionary<string, BigDouble> ScaledInputs { get; }
        BigDouble GetNumberProperty(string property);
        bool GetBooleanProperty(string property);
        string GetStringProperty(string property);
        Dictionary<string, ValueContainer> GetMapProperty(string property);
    }
}