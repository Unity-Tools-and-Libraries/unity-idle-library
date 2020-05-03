using BreakInfinity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework
{
    public class ListLiteral : ListContainer
    {
        private IList<ValueContainer> values;

        public ListLiteral(params ValueContainer[] values)
        {
            this.values = values;
        }

        public IList<ValueContainer> Get(IdleEngine engine)
        {
            return values;
        }
    }
}