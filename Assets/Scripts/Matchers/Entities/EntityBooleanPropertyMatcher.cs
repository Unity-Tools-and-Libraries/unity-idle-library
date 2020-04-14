using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework
{
    public class EntityBooleanPropertyMatcher : BooleanPropertyMatcher
    {
        public EntityBooleanPropertyMatcher(string entityKey, string entityProperty, BooleanContainer toCompareAgainst) : base(new EntityBooleanPropertyReference(entityKey, entityProperty), toCompareAgainst)
        {
        }

        public EntityBooleanPropertyMatcher(string entityKey, string entityProperty, bool toCompareAgainst) : this(entityKey, entityProperty, Literal.Of(toCompareAgainst))
        {

        }
    }
}