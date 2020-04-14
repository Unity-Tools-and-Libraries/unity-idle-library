
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework
{
    public interface ListContainer : ValueContainer, IEnumerable<ValueContainer>
    {
        void Add(ValueContainer value);
        IList<ValueContainer> Get(IdleEngine engine);
        bool Remove(ValueContainer value);
    }

    public static class ListContainerExtensions
    {
        public static ListContainer AsList(this ValueContainer value)
        {
            if(typeof(ListContainer).IsAssignableFrom(value.GetType()))
            {
                return (ListContainer)value;
            }
            return Literal.Containing();
        }
    }
}