using IdleFramework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace IdleFramework
{
    public class EntitiesWithTypeReference : ListContainer
    {
        private readonly string type;

        public EntitiesWithTypeReference(string type)
        {
            this.type = type;
        }

        public IList<ValueContainer> Get(IdleEngine engine)
        {
            return engine.GetEntitiesWithType(type).Select(e => (ValueContainer)e).ToList();
        }
    }
}