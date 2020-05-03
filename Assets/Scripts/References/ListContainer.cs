
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework
{
    public interface ListContainer : ValueContainer
    {
        IList<ValueContainer> Get(IdleEngine engine);
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