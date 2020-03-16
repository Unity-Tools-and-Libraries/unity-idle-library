using System.Collections.Generic;

namespace IdleFramework
{
    public interface SingletonEntityProperties
    {
        /*
         * The unique identifier for the singleton type.
         */
        string SingletonTypeKey { get; }
    }
}