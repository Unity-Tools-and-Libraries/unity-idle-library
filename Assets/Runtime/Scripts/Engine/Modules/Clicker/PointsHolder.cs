using BreakInfinity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker
{
    public class PointsHolder
    {
        public BigDouble Quantity { get; set; }
        public BigDouble TotalIncome { get; set; }
        public BigDouble ClickIncome { get; set; } = 1;
    }
}