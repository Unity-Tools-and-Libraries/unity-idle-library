using BreakInfinity;
using Newtonsoft.Json;
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
        [JsonIgnore]
        public BigDouble ClickIncome => this.ClickIncomeBase * this.ClickIncomeMultiplier;

        public BigDouble ClickIncomeBase { get; set; } = 1;
        public BigDouble ClickIncomeMultiplier { get; set; } = 1;

        [JsonConstructor]
        public ResourceHolder()
        {

        }

        public ResourceHolder(BigDouble startingQuantity)
        {
            Quantity = startingQuantity;
        }

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
            Quantity += quantity;
            return Quantity;
        }
    }
}