using System.Collections.Generic;
namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg.Events
{
    public class ItemAddedEvent : RpgCharacterModifierEvent<RpgItem>
    {
        public const string EventName = "itemAdded";
        public RpgItem item { get; }

        public ItemAddedEvent(RpgItem item)
        {
            this.item = item;
        }

        public override Dictionary<string, object> GetScriptingProperties()
        {
            return new Dictionary<string, object>()
            {
                { "item", item }
            };
        }
    }

    public class ItemRemovedEvent : RpgCharacterModifierEvent<RpgItem>
    {
        public const string EventName = "itemAdded";
        public RpgItem item { get; }

        public ItemRemovedEvent(RpgItem item)
        {
            this.item = item;
        }

        public override Dictionary<string, object> GetScriptingProperties()
        {
            return new Dictionary<string, object>()
            {
                { "item", item }
            };
        }
    }
}