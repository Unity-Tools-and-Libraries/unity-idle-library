using IdleFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework.State.Matchers
{
    public class EntityAvailableMatcher : EntityBooleanPropertyMatcher
    {
        public EntityAvailableMatcher(string entityKey) : base(entityKey, "Available", true)
        {
        }
    }
}