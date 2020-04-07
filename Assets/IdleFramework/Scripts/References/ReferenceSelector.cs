using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework
{
    /*
     * Wraps a query to get a value from the engine.
     */
    public interface ReferenceSelector<T>
    {
        T GetValue(IdleEngine engine);
    }
}