using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Events;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;
using System;
using System.Linq;
using System.Collections.Generic;
using static io.github.thisisnozaku.idle.framework.ValueContainer;

namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg
{
    public class Character : EventSource<ListenerSubscription>, IUserDataType
    {
        private ValueContainer underlying;

        internal Character(ValueContainer targetContainer)
        {
            this.underlying = targetContainer;
            underlying.GetProperty(Attributes.ID, IdleEngine.GetOperationType.GET_OR_CREATE).Set(underlying.Id);
            if (underlying.GetProperty(Attributes.ABILITIES, IdleEngine.GetOperationType.GET_OR_CREATE).ValueAsList() == null)
            {
                underlying.GetProperty(Attributes.ABILITIES).Set(new List<ValueContainer>());
            }
            if (underlying.GetProperty(Attributes.STATUSES, IdleEngine.GetOperationType.GET_OR_CREATE).ValueAsMap() == null)
            {
                underlying.GetProperty(Attributes.STATUSES).Set(new Dictionary<string, ValueContainer>());
            }
            underlying.GetProperty(Attributes.MAXIMUM_HEALTH, IdleEngine.GetOperationType.GET_OR_CREATE);
            underlying.GetProperty(Attributes.CURRENT_HEALTH, IdleEngine.GetOperationType.GET_OR_CREATE);
            underlying.GetProperty(Attributes.ACTION, IdleEngine.GetOperationType.GET_OR_CREATE).Set("");
            underlying.GetProperty(Attributes.ACTION_METER, IdleEngine.GetOperationType.GET_OR_CREATE).Set(0);
            underlying.GetProperty(Attributes.LEVEL, IdleEngine.GetOperationType.GET_OR_CREATE).Set(0);
            underlying.GetProperty(Attributes.ITEMS, IdleEngine.GetOperationType.GET_OR_CREATE).Set(new Dictionary<string, ValueContainer>());
            underlying.GetProperty(Attributes.ITEM_SLOTS, IdleEngine.GetOperationType.GET_OR_CREATE).Set(RpgModule.defaultItemSlots
                .ToDictionary(x => x.Key, x => targetContainer.Engine.CreateValueContainer(x.Value)));
            targetContainer.SetUpdater("CharacterUpdateMethod");
        }

        public object Act(IdleEngine engine, params object[] args)
        {
            return null;
        }
        public string Id { get { return underlying.Id; } }
        public BigDouble Level { get { return underlying.GetProperty(Attributes.LEVEL, IdleEngine.GetOperationType.GET_OR_CREATE); } set { underlying.GetProperty(Attributes.LEVEL, IdleEngine.GetOperationType.GET_OR_CREATE).Set(value); } }

        public bool AddItem(ItemInstance item)
        {
            var itemSlots = item.UsedSlots;
            foreach (var slot in itemSlots)
            {
                var existing = underlying.GetProperty(Attributes.ITEMS).GetProperty(slot, IdleEngine.GetOperationType.GET_OR_CREATE).ValueAsList();
                if ((existing != null ? existing.Count() : 0) >= underlying.GetProperty(Attributes.ITEM_SLOTS).GetProperty(slot).ValueAsNumber())
                {
                    return false;
                }
            }
            foreach (var slot in itemSlots)
            {
                var existing = underlying.GetProperty(Attributes.ITEMS).GetProperty(slot, IdleEngine.GetOperationType.GET_OR_CREATE).ValueAsList();
                if(existing == null)
                {
                    existing = underlying.GetProperty(Attributes.ITEMS).GetProperty(slot, IdleEngine.GetOperationType.GET_OR_CREATE).Set(new List<ValueContainer>());
                }
                existing.Add(item);
            }
            return true;
        }

        public BigDouble CurrentHealth { get { return underlying.GetProperty(Attributes.CURRENT_HEALTH, IdleEngine.GetOperationType.GET_OR_CREATE); } set { underlying.GetProperty(Attributes.CURRENT_HEALTH, IdleEngine.GetOperationType.GET_OR_CREATE).Set(value); } }

        public IList<ItemInstance> GetItems(string slot)
        {
            var forSlot = underlying.GetProperty(Attributes.ITEMS).GetProperty(slot, IdleEngine.GetOperationType.GET_OR_CREATE);
            return (forSlot.ValueAsList() != null ? forSlot.ValueAsList() : forSlot.Set(new List<ValueContainer>())).Select(x => x.AsItem()).ToList();
        }

        public BigDouble MaximumHealth { get { return underlying.GetProperty(Attributes.MAXIMUM_HEALTH, IdleEngine.GetOperationType.GET_OR_CREATE); } set { underlying.GetProperty(Attributes.MAXIMUM_HEALTH, IdleEngine.GetOperationType.GET_OR_CREATE).Set(value); } }
        public BigDouble Damage { get { return underlying.GetProperty(Attributes.DAMAGE, IdleEngine.GetOperationType.GET_OR_CREATE); } set { underlying.GetProperty(Attributes.DAMAGE, IdleEngine.GetOperationType.GET_OR_CREATE).Set(value); } }
        public BigDouble ActionMeter { get { return underlying.GetProperty(Attributes.ACTION_METER, IdleEngine.GetOperationType.GET_OR_CREATE); } set { underlying.GetProperty(Attributes.ACTION_METER, IdleEngine.GetOperationType.GET_OR_CREATE).Set(value); } }
        public BigDouble Accuracy { get { return underlying.GetProperty(Attributes.ACCURACY, IdleEngine.GetOperationType.GET_OR_CREATE); } set { underlying.GetProperty(Attributes.ACCURACY, IdleEngine.GetOperationType.GET_OR_CREATE).Set(value); } }
        public BigDouble Evasion { get { return underlying.GetProperty(Attributes.EVASION, IdleEngine.GetOperationType.GET_OR_CREATE); } set { underlying.GetProperty(Attributes.EVASION, IdleEngine.GetOperationType.GET_OR_CREATE).Set(value); } }
        public BigDouble Defense { get { return underlying.GetProperty(Attributes.DEFENSE, IdleEngine.GetOperationType.GET_OR_CREATE); } set { underlying.GetProperty(Attributes.DEFENSE, IdleEngine.GetOperationType.GET_OR_CREATE).Set(value); } }
        public BigDouble Penetration { get { return underlying.GetProperty(Attributes.PENETRATION, IdleEngine.GetOperationType.GET_OR_CREATE); } set { underlying.GetProperty(Attributes.PENETRATION, IdleEngine.GetOperationType.GET_OR_CREATE).Set(value); } }
        public BigDouble ActionMeterSpeed { get { return underlying.GetProperty(Attributes.PENETRATION, IdleEngine.GetOperationType.GET_OR_CREATE); } set { underlying.GetProperty(Attributes.PENETRATION, IdleEngine.GetOperationType.GET_OR_CREATE).Set(value); } }
        public bool Targetable { get { return underlying.GetProperty(Attributes.TARGETABLE, IdleEngine.GetOperationType.GET_OR_CREATE); } set { underlying.GetProperty(Attributes.TARGETABLE, IdleEngine.GetOperationType.GET_OR_CREATE).Set(value); } }
        public BigDouble Party { get { return underlying.GetProperty(Attributes.PARTY, IdleEngine.GetOperationType.GET_OR_CREATE); } set { underlying.GetProperty(Attributes.PARTY, IdleEngine.GetOperationType.GET_OR_CREATE).Set(value); } }
        public IDictionary<string, ValueContainer> Statuses { get { return underlying.GetProperty(Attributes.STATUSES, IdleEngine.GetOperationType.GET_OR_CREATE).ValueAsMap(); } set { underlying.GetProperty(Attributes.EVASION, IdleEngine.GetOperationType.GET_OR_CREATE).Set(value); } }
        public bool IsAlive { get { return underlying.GetProperty(Attributes.CURRENT_HEALTH, IdleEngine.GetOperationType.GET_OR_CREATE).ValueAsNumber() > 0; } }
        public string Icon { get { return underlying.GetProperty(Attributes.ICON, IdleEngine.GetOperationType.GET_OR_CREATE).ValueAsString(); } set { underlying.GetProperty(Attributes.ICON, IdleEngine.GetOperationType.GET_OR_CREATE).Set(value); } }
        public string Action { get { return underlying.GetProperty(Attributes.ACTION, IdleEngine.GetOperationType.GET_OR_CREATE).ValueAsString(); } set { underlying.GetProperty(Attributes.ACTION, IdleEngine.GetOperationType.GET_OR_CREATE).Set(value); } }
        public IDictionary<string, BigDouble> ItemSlots
        {
            get { return underlying.GetProperty(Attributes.ITEM_SLOTS, IdleEngine.GetOperationType.GET_OR_CREATE).ValueAsMap().ToDictionary(x => x.Key, x => x.Value.ValueAsNumber()); }
            set { underlying.GetProperty(Attributes.ITEM_SLOTS, IdleEngine.GetOperationType.GET_OR_CREATE).Set(value); }
        }

        public void NotifyImmediately(string eventName, IDictionary<string, object> notificationContext = null, params object[] arguments)
        {
            ((EventSource<ListenerSubscription>)underlying).NotifyImmediately(eventName, notificationContext, arguments);
        }

        public void NotifyImmediately(string eventName, params object[] arguments)
        {
            NotifyImmediately(eventName, null, arguments);
        }

        public void AddAbility(AbilityDefinition ability)
        {
            underlying.AddModifier(ability);
            underlying.GetProperty(Character.Attributes.ABILITIES).ValueAsList().Add(underlying.Engine.CreateValueContainer(ability.Id));
        }

        public ListenerSubscription Subscribe(string subscriber, string eventName, string handlerName, bool ephemeral = false)
        {
            return ((EventSource<ListenerSubscription>)underlying).Subscribe(subscriber, eventName, handlerName, ephemeral);
        }

        public ValueContainer GetProperty(string property)
        {
            return underlying.GetProperty(property);
        }

        public void Unsubscribe(ListenerSubscription subscription)
        {
            ((EventSource<ListenerSubscription>)underlying).Unsubscribe(subscription);
        }

        public void Unsubscribe(string subscriber, string eventName)
        {
            ((EventSource<ListenerSubscription>)underlying).Unsubscribe(subscriber, eventName);
        }

        public static implicit operator ValueContainer(Character character)
        {
            return character.underlying;
        }

        private void UpdateActionMeter(IdleEngine engine, BigDouble time)
        {
            BigDouble actionMeter = underlying.GetProperty(Character.Attributes.ACTION_METER).ValueAsNumber() + time;
            if (actionMeter >= engine.GetProperty("configuration.action_meter_required_to_act").ValueAsNumber())
            {
                actionMeter = 0;
                underlying.NotifyImmediately(CharacterActedEvent.EventName, null, this);
            }
            underlying.GetProperty(Character.Attributes.ACTION_METER).Set(actionMeter);
        }

        private void UpdateStatusDurations(IdleEngine engine, BigDouble time)
        {
            var toUpdate = new Dictionary<string, ValueContainer>(Statuses);
            foreach (var status in toUpdate)
            {
                BigDouble currentTime = status.Value.ValueAsNumber();
                status.Value.Set(currentTime - time);
                if (currentTime - time <= 0)
                {
                    engine.RemoveStatus(this, status.Key);
                }
            }
        }
        [HandledEvent(typeof(ValueContainerWillUpdateEvent))]
        public static object UpdateMethod(IdleEngine engine, params object[] args)
        {
            var container = (args[0] as ValueContainer);
            if (engine.GetProperty(RpgModule.Properties.ActionPhase).ValueAsString() == "combat")
            {
                var character = container.AsCharacter();
                character.UpdateActionMeter(engine, (BigDouble)args[1]);
                character.UpdateStatusDurations(engine, (BigDouble)args[1]);
            }
            return container.ValueAsMap();
        }

        public DynValue Index(Script script, DynValue index, bool isDirectIndexing)
        {
            return DynValue.FromObject(script, underlying.GetProperty(index.CastToString()));
        }

        public bool SetIndex(Script script, DynValue index, DynValue value, bool isDirectIndexing)
        {
            underlying.GetProperty(index.CastToString(), IdleEngine.GetOperationType.GET_OR_CREATE).Set(value.ToObject());
            return true;
        }

        public DynValue MetaIndex(Script script, string metaname)
        {
            throw new NotImplementedException();
        }

        public static class Attributes
        {
            public const string ID = "id";
            public const string ACTION = "action";
            public const string ITEM_SLOTS = "item_slots";
            public const string ITEMS = "items";
            public const string ACTION_METER_SPEED = "action_meter_speed";
            public const string PARTY = "party";
            public const string LEVEL = "level";
            public const string CURRENT_HEALTH = "current_health";
            public const string MAXIMUM_HEALTH = "maximum_health";
            public const string DAMAGE = "damage";
            public const string ACTION_METER = "action_meter";
            public const string ABILITIES = "abilities";
            public const string STATUSES = "statuses";
            public const string ACCURACY = "accuracy";
            public const string EVASION = "evasion";
            public const string DEFENSE = "defense";
            public const string PENETRATION = "penetration";
            public const string TARGETABLE = "targetable";
            public const string ICON = "icon";
            public const string XP = "xp";
        }

        public static class Actions
        {
            public const string FIGHT = "FIGHT";
            public const string REINCARNATING = "REINCARNATING";
        }
    }
}