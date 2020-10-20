using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework
{
    public interface CanSnapshot<T>
    {
        T GetSnapshot();
        void RestoreFromSnapshot(IdleEngine engine, T snapshot);
    }
}