using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace io.github.thisisnozaku.idle.framework
{
    public interface CanSnapshot<T>
    {
        T GetSnapshot();
        void RestoreFromSnapshot(IdleEngine engine, T snapshot, ValueContainer parent = null);
    }
}