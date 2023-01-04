using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace io.github.thisisnozaku.idle.framework.Engine.State
{
    public interface CommandReceiver
    {
        public void EvaluateCommand(string command);
    }
}