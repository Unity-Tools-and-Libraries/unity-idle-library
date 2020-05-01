using IdleFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework
{
    public class OfflineFastForwardPolicy : IOfflinePolicy
    {
        public void Apply(IdleEngine engine, float amountOfTime)
        {
            engine.Update(amountOfTime);
        }
    }
}