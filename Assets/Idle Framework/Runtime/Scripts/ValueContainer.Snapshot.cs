using System;
using System.Collections.Generic;
using System.Linq;

namespace io.github.thisisnozaku.idle.framework
{
    // TODO: Move into own file
    public class ValueContainerSnapshot
    {
        public string Type;
        public string Path;
        public object Value;
        public string UpdateMethod;
        public Dictionary<string, List<string>> Listeners;

        public ValueContainerSnapshot()
        {

        }

        public ValueContainerSnapshot(string path, object value, string updateMethod, Dictionary<string, List<string>> listeners)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }
            Path = path;
            Type = ValueContainer.DetermineType(value);
            Value = Type == "map" ? (value as IDictionary<string, ValueContainer>)
                .ToDictionary(x =>
                {
                    return x.Key;
                }, x =>
                {
                    return x.Value.GetSnapshot();
                }) :
                (value != null ? value : "null");

            Listeners = listeners;
            UpdateMethod = updateMethod;
        }
    }
}