using IdleFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework.State.Matchers
{
    public class EntityEnabledMatcher : EntityBooleanPropertyMatcher
    {
        public EntityEnabledMatcher(string entityKey) : base(entityKey, "Enabled", true)
        {
        }
    }
}