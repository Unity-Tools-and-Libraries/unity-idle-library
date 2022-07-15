using BreakInfinity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace io.github.thisisnozaku.idle.framework.Engine
{
    /*
     * Class representing a duration. Stored both the initial and current remaining time.
     */
    public class Duration
    {
        public BigDouble RemainingTime { get; set; }
        public BigDouble InitialTime { get; }
        public Duration(BigDouble initialTime)
        {
            RemainingTime = InitialTime = initialTime;
        }
    }
}