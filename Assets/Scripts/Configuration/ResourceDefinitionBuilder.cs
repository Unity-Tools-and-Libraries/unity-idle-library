using BreakInfinity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace io.github.thisisnozaku.idle.framework.Configuration
{
    public class ResourceDefinitionBuilder
    {
        private string id;
        private BigDouble baseIncome;
        public string Id => id;
        public ResourceDefinitionBuilder(string id)
        {
            this.id = id;
        }

        public ResourceDefinitionBuilder WithBaseIncome(BigDouble baseIncome)
        {
            this.baseIncome = baseIncome;
            return this;
        }

        public ResourceDefinition Build()
        {
            return new ResourceDefinition(id, baseIncome);
        }
    }
}