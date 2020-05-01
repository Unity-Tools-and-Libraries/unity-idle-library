using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework
{
    /*
     * A policy for how to handle offline time.
     */
    public interface IOfflinePolicy
    {
        void Apply(IdleEngine engine, float amountOfTime);
    }
}