using System.Collections.Generic;

namespace io.github.thisisnozaku.idle.framework.Definitions
{
    public interface IDefinition
    {
        string Id { get; }
        IDictionary<string, object> Properties { get; }
    }
}