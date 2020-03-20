using BreakInfinity;
using System;
using System.Collections.Generic;

namespace IdleFramework
{
    /*
     * A state comparison matcher that checks that the specified entity exists and compares the value it has for the specified property and sub property.
     */
    public abstract class EntityPropertyMatcher : StateMatcher
    {
        private readonly string entityKey;
        private readonly string entityProperty;
        private readonly string entitySubproperty;

        public EntityPropertyMatcher(string entityKey, string entityProperty, string entitySubproperty)
        {
            this.entityKey = entityKey.ToLower();
            this.entityProperty = entityProperty.ToLower();
            this.entitySubproperty = entitySubproperty.ToLower();
        }

        public string EntityProperty => entityProperty;

        public string EntitySubproperty => entitySubproperty;

        public string EntityKey => entityKey;

        public abstract bool Matches(IdleEngine toCheck);
    }       

}