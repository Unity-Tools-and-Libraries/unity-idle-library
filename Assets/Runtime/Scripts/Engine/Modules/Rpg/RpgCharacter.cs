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

namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg
{
    public class RpgCharacter : Entity
    {
        public RpgCharacter(IdleEngine engine) : base(engine)
        {
            Statuses = new Dictionary<long, Duration>();
            ItemSlots = new Dictionary<string, RpgItem[]>();
            foreach(var defaultSlot in engine.GetConfiguration<Dictionary<string, int>>("characterItemSlots"))
            {
                ItemSlots[defaultSlot.Key] = new RpgItem[defaultSlot.Value];
            }
            Abilities = new Dictionary<long, AbilityDefinition>();
        }

        public BigDouble Level { get; set; }
        public BigDouble CurrentHealth { get; set; }
        public BigDouble MaximumHealth { get; set; }
        public BigDouble Damage { get; set; }
        public BigDouble ActionMeter { get; set; }
        public BigDouble Accuracy { get; set; }
        public BigDouble Evasion { get; set; }
        public BigDouble Defense { get; set; }
        public BigDouble Penetration { get; set; }
        public BigDouble Precision { get; set; }
        public BigDouble Resilience { get; set; }
        public BigDouble ActionMeterSpeed { get; set; }
        public BigDouble CriticalHitDamageMultiplier { get; set; }
        public bool Targetable { get; set; }
        public BigDouble Party { get; set; }
        public bool IsAlive { get; set; }
        public string Action { get; set; }
        public BigDouble CriticalHitChance { get; set; }
        public BigDouble Xp { get; set; }
        public BigDouble Gold { get; set; }
        public Dictionary<long, AbilityDefinition> Abilities { get; set; }
        public Dictionary<string, RpgItem[]> ItemSlots { get; set; }
        public IDictionary<long, Duration> Statuses { get; set; }

        private List<EntityModifier<Entity>> modifiers = new List<EntityModifier<Entity>>();

        public void Act(IdleEngine engine)
        {
            var target = engine.GetRandomTarget(Party);
            if (target != null)
            {
                engine.MakeAttack(this, target);
            }
            var ev = new CharacterActedEvent(this);
            Emit(CharacterActedEvent.EventName, ev);
            engine.Emit(CharacterActedEvent.EventName, ev);
        }

        public BigDouble InflictDamage(BigDouble attackDamage, RpgCharacter source)
        {
            var startingHealth = CurrentHealth;
            BigDouble actualDamage = BigDouble.Min(attackDamage, CurrentHealth);

            if (CurrentHealth <= actualDamage)
            {
                Kill(source);
            } else
            {
                CurrentHealth -= actualDamage;
            }

            var damageInflicted = new DamageInflictedEvent(source, attackDamage, this);
            if (source != null)
            {
                source.Emit(DamageInflictedEvent.EventName, damageInflicted);
                Engine.Emit(DamageInflictedEvent.EventName, damageInflicted);
            }

            var damagedTaken = new DamageTakenEvent(source, attackDamage, this);
            Emit(DamageTakenEvent.EventName, damagedTaken);
            Engine.Emit(DamageTakenEvent.EventName, damagedTaken);

            
            return actualDamage;
        }

        public void AddAbility(AbilityDefinition ability)
        {
            if(!Abilities.ContainsKey(ability.Id))
            {
                AddModifier(ability);
                Abilities[ability.Id] = ability;
            }
        }

        public void RemoveAbility(AbilityDefinition ability)
        {
            if (Abilities.ContainsKey(ability.Id))
            {
                RemoveModifier(ability);
                Abilities.Remove(ability.Id);
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
            }
            Statuses[status.Id] = new Duration(duration);
        }

        public void RemoveStatus(CharacterStatus statusDefinition)
        {
            if (Statuses.ContainsKey(statusDefinition.Id))
            {
                RemoveModifier(statusDefinition);
                Statuses.Remove(statusDefinition.Id);
            }
        }

        public bool AddItem(RpgItem item)
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
                } else
                {
                    Statuses[statusId].RemainingTime = currentTime;
                }
            }
        }
        
        protected override void CustomUpdate(IdleEngine engine, float deltaTime)
        {
            if ((string)engine.GetProperty(RpgModule.Properties.ActionPhase) == "combat")
            {
                UpdateActionMeter(deltaTime);
                UpdateStatusDurations(deltaTime);
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
            var diedEvent = new CharacterDiedEvent(this, killer);
            Emit(CharacterDiedEvent.EventName, diedEvent);
            Engine.Emit( CharacterDiedEvent.EventName, diedEvent);
        }

        public static class Attributes
        {
            public const string ABILITIES = "abilities";
            public const string ACCURACY = "accuracy";
            public const string ACTION = "action";
            public const string ACTION_METER = "action_meter";
            public const string ACTION_SPEED = "action_meter_speed";
            public const string CRITICAL_DAMAGE_MULTIPLIER = "critical_damage_multiplier";
            public const string CRITICAL_HIT_CHANCE = "critical_hit_chance";
            public const string CURRENT_HEALTH = "current_health";
            public const string DAMAGE = "damage";
            public const string DEFENSE = "defense";
            public const string EVASION = "evasion";
            public const string GOLD = "gold";
            public const string ID = "id";
            public const string ITEM_SLOTS = "item_slots";
            public const string ITEMS = "items";
            public const string LEVEL = "level";
            public const string MAXIMUM_HEALTH = "maximum_health";
            public const string PARTY = "party";
            public const string PENETRATION = "penetration";
            public const string PRECISION = "precision";
            public const string RESILIENCE = "resilience";
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