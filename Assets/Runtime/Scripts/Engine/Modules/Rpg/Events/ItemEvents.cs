using System.Collections.Generic;
namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg.Events
{
    public class ItemAddedEvent : RpgCharacterModifierEvent<CharacterItem>
    {
        public const string EventName = "itemAdded";
        public CharacterItem item { get; }

        public ItemAddedEvent(CharacterItem item)
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

    public class ItemRemovedEvent : RpgCharacterModifierEvent<CharacterItem>
    {
        public const string EventName = "itemAdded";
        public CharacterItem item { get; }

        public ItemRemovedEvent(CharacterItem item)
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