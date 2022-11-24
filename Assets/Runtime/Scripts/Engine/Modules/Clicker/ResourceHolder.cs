using BreakInfinity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker
{
    public class ResourceHolder
    {
        public BigDouble Quantity { get; set; }
        public BigDouble TotalIncome { get; set; }
        public BigDouble ClickIncome { get; set; } = 1;

        public bool Spend(BigDouble quantity)
        {
            if (Quantity >= quantity)
            {
                Quantity -= quantity;
                return true;
            }
            return false;
        }

        public BigDouble Change(BigDouble quantity)
        {
            Quantity = quantity;
            return Quantity;
        }
    }
}