using BreakInfinity;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;
using System;
using System.Linq;
using System.Collections.Generic;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg.Combat;
using System.Dynamic;
using io.github.thisisnozaku.idle.framework.Engine.Persistence;
using io.github.thisisnozaku.idle.framework.Events;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg.Events;
using Newtonsoft.Json;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg.Configuration;

namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg
{
    public class RpgCharacter : Entity, IHasResources
    {
        public RpgCharacter(IdleEngine engine, double id, Dictionary<string, BigDouble> resources = null) : base(engine, id)
        {
            Statuses = new Dictionary<double, Duration>();
            ItemSlots = new Dictionary<string, CharacterItem[]>();
            if (engine != null) // Engine can be null when deserializing and is set after construction.
            {
                foreach (var defaultSlot in engine.GetConfiguration<Dictionary<string, int>>("characterItemSlots"))
                {
                    ItemSlots[defaultSlot.Key] = new CharacterItem[defaultSlot.Value];
                }
            }
            Abilities = new Dictionary<double, CharacterAbility>();
            OnEventTriggers = new Dictionary<string, List<string>>();
            Resources = resources != null ? resources.ToDictionary(r => r.Key, r =>
            {
                var rh = new ResourceHolder();
                rh.Quantity = r.Value;
                return rh;
            }) : new Dictionary<string, ResourceHolder>();
        }
        /*
         * This is the path in the configuration to the to-hit determination script for this character.
         */
        public string ToHitScript { get; set; }
        /*
         * The level of this character. The higher the level, the more powerful the character.
         */
        public virtual BigDouble Level { get; set; }
        /*
         * The amount of health this character has. Once health is reduced to 0, the character dies.
         */
        public virtual BigDouble CurrentHealth { get; set; }
        /*
         * The maximum amount of health this character has.
         */
        public NumericAttribute MaximumHealth = new NumericAttribute(0);
        public NumericAttribute Damage = new NumericAttribute(0);
        public virtual BigDouble ActionMeter { get; set; }
        public NumericAttribute Accuracy = new NumericAttribute(0);
        public NumericAttribute Evasion = new NumericAttribute(0);
        public NumericAttribute Defense = new NumericAttribute(0);
        public NumericAttribute Penetration = new NumericAttribute(0);
        public NumericAttribute Precision = new NumericAttribute(0);
        public NumericAttribute Resilience = new NumericAttribute(0);
        public NumericAttribute ActionMeterSpeed = new NumericAttribute(0);
        public NumericAttribute CriticalHitDamageMultiplier = new NumericAttribute(0);
        public NumericAttribute Regeneration = new NumericAttribute(0);
        public NumericAttribute ResurrectionMultiplier = new NumericAttribute(0);
        public virtual bool Targetable { get; set; }
        public virtual BigDouble Party { get; set; }
        public virtual bool IsAlive => CurrentHealth > 0;
        public string Action { get; set; }
        public readonly NumericAttribute CriticalHitChance = new NumericAttribute(0);
        public virtual Dictionary<double, CharacterAbility> Abilities { get; set; }
        public virtual Dictionary<string, CharacterItem[]> ItemSlots { get; set; }
        public virtual Dictionary<double, Duration> Statuses { get; set; }
        public virtual Dictionary<string, List<string>> OnEventTriggers { get; private set; }
        private Dictionary<string, ResourceHolder> Resources;
        public string NextAction;

        public void Act(IdleEngine engine)
        {
            var target = engine.GetRandomTarget(Party);
            if (target != null)
            {
                engine.MakeAttack(this, target);
            }
            var ev = new CharacterActedEvent(this, NextAction);
            Emit(CharacterActedEvent.EventName, ev);
            engine.Emit(CharacterActedEvent.EventName, ev);
        }

        public BigDouble InflictDamage(BigDouble attackDamage, RpgCharacter source)
        {
            var startingHealth = CurrentHealth;
            BigDouble actualDamage = BigDouble.Min(attackDamage, CurrentHealth);

            if (CurrentHealth <= actualDamage)
            {
                actualDamage = CurrentHealth;
            }
            CurrentHealth -= actualDamage;

            var damageInflicted = new DamageInflictedEvent(source, attackDamage, this);
            if (source != null)
            {
                source.Emit(DamageInflictedEvent.EventName, damageInflicted);
            }
            Engine.Emit(DamageInflictedEvent.EventName, damageInflicted);

            var damagedTaken = new DamageTakenEvent(source, attackDamage, this);
            Emit(DamageTakenEvent.EventName, damagedTaken);
            Engine.Emit(DamageTakenEvent.EventName, damagedTaken);

            if(CurrentHealth == 0)
            {
                Kill(source);
            }

            return actualDamage;
        }

        public void AddAbility(CharacterAbility ability)
        {
            if (!Abilities.ContainsKey(ability.Id))
            {
                AddModifier(ability);
                Abilities[ability.Id] = ability;
                if (ability.EventTriggers != null)
                {
                    foreach (var @event in ability.EventTriggers)
                    {
                        List<string> triggers;
                        if (!OnEventTriggers.TryGetValue(@event.Key, out triggers))
                        {
                            triggers = new List<string>();
                            OnEventTriggers[@event.Key] = triggers;
                        }
                        triggers.AddRange(@event.Value);
                    }
                }
                Emit(AbilityAddedEvent.EventName, new AbilityAddedEvent(ability));
            }
        }

        public void RemoveAbility(CharacterAbility ability)
        {
            if (Abilities.ContainsKey(ability.Id))
            {
                RemoveModifier(ability);
                Abilities.Remove(ability.Id);
                if (ability.EventTriggers != null)
                {
                    foreach (var @event in ability.EventTriggers)
                    {
                        List<string> triggers;
                        if (OnEventTriggers.TryGetValue(@event.Key, out triggers))
                        {
                            foreach (var effect in @event.Value)
                            {
                                triggers.Remove(effect);
                            }
                        }
                    }
                }
                Emit(AbilityRemovedEvent.EventName, new AbilityRemovedEvent(ability));
            }
        }

        public void AddStatus(CharacterStatus status, BigDouble duration)
        {
            if (status == null)
            {
                throw new ArgumentNullException("status");
            }
            if (!Engine.GetStatuses().ContainsKey(status.Id))
            {
                throw new InvalidOperationException("Engine is not aware of a status with id " + status.Id);
            }
            if (!Statuses.ContainsKey(status.Id))
            {
                AddModifier(status);
                foreach (var @event in status.EventTriggers)
                {
                    List<string> triggers;
                    if (!OnEventTriggers.TryGetValue(@event.Key, out triggers))
                    {
                        triggers = new List<string>();
                        OnEventTriggers[@event.Key] = triggers;
                    }
                    triggers.AddRange(@event.Value);
                }
                Emit(StatusAddedEvent.EventName, new StatusAddedEvent(status, duration));
            }
            Statuses[status.Id] = new Duration(duration);
        }

        public void RemoveStatus(CharacterStatus status)
        {
            if (Statuses.ContainsKey(status.Id))
            {
                RemoveModifier(status);
                Statuses.Remove(status.Id);
                foreach (var @event in status.EventTriggers)
                {
                    List<string> triggers;
                    if (OnEventTriggers.TryGetValue(@event.Key, out triggers))
                    {
                        foreach (var effect in @event.Value)
                        {
                            OnEventTriggers[@event.Key].Remove(effect);
                        }
                    }
                }
                Emit(StatusRemovedEvent.EventName, new StatusRemovedEvent(status));
            }
        }

        public bool AddItem(CharacterItem item)
        {
            var itemSlots = item.UsedSlots;
            foreach (var slot in itemSlots)
            {
                // If the last space isn't empty, the slot is full
                if (ItemSlots[slot][ItemSlots[slot].Length - 1] != null)
                {
                    return false;
                }
            }
            foreach (var slot in itemSlots)
            {
                for (int i = 0; i < ItemSlots[slot].Length; i++)
                {
                    if (ItemSlots[slot][i] == null)
                    {
                        ItemSlots[slot][i] = item;
                        break;
                    }
                }
            }
            if (item.EventTriggers != null)
            {
                foreach (var @event in item.EventTriggers)
                {
                    List<string> triggers;
                    if (!OnEventTriggers.TryGetValue(@event.Key, out triggers))
                    {
                        triggers = new List<string>();
                        OnEventTriggers[@event.Key] = triggers;
                    }
                    triggers.AddRange(@event.Value);
                }
            }
            AddModifier(item);
            Emit(ItemAddedEvent.EventName, new ItemAddedEvent(item));
            return true;
        }

        public bool RemoveItem(CharacterItem item)
        {
            bool removed = false;
            foreach (var slot in item.UsedSlots)
            {
                for (int i = 0; i < ItemSlots[slot].Length; i++)
                {
                    if (ItemSlots[slot][i] == item)
                    {
                        ItemSlots[slot][i] = null;
                        removed = true;
                    }
                }
            }
            if (item.EventTriggers != null)
            {
                foreach (var @event in item.EventTriggers)
                {
                    List<string> triggers;
                    if (OnEventTriggers.TryGetValue(@event.Key, out triggers))
                    {
                        foreach(var effect in @event.Value)
                        {
                            triggers.Remove(effect);
                        }
                    }
                }
            }
            RemoveModifier(item);
            if(removed)
            {
                Emit(ItemRemovedEvent.EventName, new ItemRemovedEvent(item));
            }
            return true;
        }



        private void UpdateActionMeter(BigDouble time)
        {

            BigDouble actionMeter = ActionMeter + time;
            if (actionMeter >= (BigDouble)Engine.GetConfiguration()["action_meter_required_to_act"])
            {
                Engine.Logging.Log(UnityEngine.LogType.Log, () => String.Format("Character {0} is acting", 1), "character.combat");
                actionMeter = 0;
                this.Act(Engine);
            }
            ActionMeter = actionMeter;
        }

        private void UpdateStatusDurations(BigDouble time)
        {
            var toUpdate = Statuses.Keys.ToArray();
            foreach (var statusId in toUpdate)
            {
                BigDouble currentTime = Statuses[statusId].RemainingTime - time;

                if (currentTime <= 0)
                {
                    RemoveStatus(Engine.GetStatuses()[statusId]);
                }
                else
                {
                    Statuses[statusId].RemainingTime = currentTime;
                }
            }
        }

        protected override void CustomUpdate(IdleEngine engine, float deltaTime)
        {
            if (engine.GetActionPhase() == "combat" &&
                engine.GetCurrentEncounter().IsActive)
            {
                UpdateActionMeter(deltaTime);
                UpdateStatusDurations(deltaTime);
            }
            BigDouble regenAmount = Regeneration.Total * deltaTime * (Action == "REINCARNATING" ? ResurrectionMultiplier.Total : 1);
            CurrentHealth += BigDouble.Min(MaximumHealth.Total, regenAmount);
            if (CurrentHealth == MaximumHealth.Total && Action == "REINCARNATING")
            {
                Action = "";
                Emit(CharacterResurrectedEvent.EventName, new CharacterResurrectedEvent(this));
            }
        }

        public void Reset()
        {
            Statuses.Clear();
            CurrentHealth = MaximumHealth;
            ActionMeter = 0;
        }

        public void Kill(RpgCharacter killer = null)
        {
            CurrentHealth = 0;
            ActionMeter = 0;
            var diedEvent = new CharacterDiedEvent(this, killer);
            Emit(CharacterDiedEvent.EventName, diedEvent);
        }

        public ResourceHolder GetResource(string id)
        {
            return Resources[id];
        }

        public NumericAttribute GetAttribute(string id)
        {
            switch(id)
            {
                case Attributes.ACCURACY:
                    return Accuracy;
                case Attributes.ACTION_SPEED:
                    return ActionMeterSpeed;
                case Attributes.CRITICAL_DAMAGE_MULTIPLIER:
                    return CriticalHitDamageMultiplier;
                case Attributes.CRITICAL_HIT_CHANCE:
                    return CriticalHitChance;
                case Attributes.DAMAGE:
                    return Damage;
                case Attributes.DEFENSE:
                    return Defense;
                case Attributes.EVASION:
                    return Evasion;
                case Attributes.MAXIMUM_HEALTH:
                    return MaximumHealth;
                case Attributes.PENETRATION:
                    return Penetration;
                case Attributes.PRECISION:
                    return Precision;
                case Attributes.REGENERATION:
                    return Regeneration;
                case Attributes.RESILIENCE:
                    return Resilience;
                case Attributes.RESURRECTION_MULTIPLIER:
                    return ResurrectionMultiplier;
            }
            return null;
        }

        public static class Attributes
        {
            public const string ABILITIES = "abilities";
            public const string ACCURACY = "accuracy"; // Affect chance to hit
            public const string ACTION = "action"; // What action the character is performing
            public const string ACTION_METER = "action_meter";
            public const string ACTION_SPEED = "action_meter_speed";
            public const string CRITICAL_DAMAGE_MULTIPLIER = "critical_damage_multiplier"; // How much damage is multiplied on a critical hit
            public const string CRITICAL_HIT_CHANCE = "critical_hit_chance";
            public const string CURRENT_HEALTH = "current_health";
            public const string DAMAGE = "damage"; 
            public const string DEFENSE = "defense"; // Reduces damage from incoming attacks
            public const string EVASION = "evasion"; // Reduces accuracy of incoming attacks
            public const string GOLD = "gold";
            public const string ID = "id";
            public const string ITEM_SLOTS = "item_slots";
            public const string ITEMS = "items";
            public const string LEVEL = "level";
            public const string MAXIMUM_HEALTH = "maximum_health";
            public const string PARTY = "party";
            public const string PENETRATION = "penetration"; // Pierces target defense
            public const string PRECISION = "precision"; // Reduces effect of target resilience
            public const string RESILIENCE = "resilience"; // Reduces chances of receiving critical hits
            public const string REGENERATION = "regeneration";
            public const string RESURRECTION_MULTIPLIER = "resurrectionMultiplier";
            public const string STATUSES = "statuses";
            public const string TARGETABLE = "targetable";
            public const string XP = "xp";
        }

        public static class Actions
        {
            public const string FIGHT = "FIGHT";
            public const string REINCARNATING = "REINCARNATING";
        }
    }
}