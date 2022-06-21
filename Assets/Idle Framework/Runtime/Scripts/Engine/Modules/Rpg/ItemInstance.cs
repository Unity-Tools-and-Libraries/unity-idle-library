using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg
{
    public class ItemInstance
    {
        private ValueContainer underlying;
        internal ItemInstance(ValueContainer valueContainer)
        {
            this.underlying = valueContainer;
            var usedSlots = underlying.GetProperty(Attributes.UsedSlots, IdleEngine.GetOperationType.GET_OR_CREATE);
            
        }

        public List<string> UsedSlots => underlying.GetProperty(Attributes.UsedSlots).ValueAsList().Select(x => x.ValueAsString()).ToList();
        
        public static implicit operator ValueContainer(ItemInstance item)
        {
            return item.underlying;
        }

        public static class Attributes
        {
            public const string UsedSlots = "used_slots";
        }
    }

    public static class ItemInstanceExtensionMethods
    {
        private static Dictionary<ValueContainer, ItemInstance> WrappedItemCache = new Dictionary<ValueContainer, ItemInstance>();
        public static ItemInstance AsItem(this ValueContainer targetContainer)
        {
            ItemInstance item;
            if (!WrappedItemCache.TryGetValue(targetContainer, out item))
            {
                item = new ItemInstance(targetContainer);
                var usedSlots = targetContainer.GetProperty(ItemInstance.Attributes.UsedSlots, IdleEngine.GetOperationType.GET_OR_CREATE);
                if(usedSlots.ValueAsList() == null)
                {
                    usedSlots.Set(new List<ValueContainer>());
                }

                WrappedItemCache[targetContainer] = item;
            }
            return item;
        }
    }
}