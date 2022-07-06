using io.github.thisisnozaku.idle.framework.Definitions;
using io.github.thisisnozaku.idle.framework.Events;
using MoonSharp.Interpreter;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg.Combat;

namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg
{
    public class RpgModule : IModule
    {
        public const string DEFAULT_XP_CALCULATION_METHOD = "return 10 * pow(2, character.level.AsNumber - 1)";
        public const string DEFAULT_GOLD_CALCULATION_METHOD = "return 10 * pow(2, character.level.AsNumber - 1)";
        public const string DEFAULT_MONSTER_SCALING_FUNCTION = "return pow(1.1, character.level.AsNumber - 1)";
        public static readonly Dictionary<string, int> defaultItemSlots = new Dictionary<string, int>()
            {
                {"head", 1 },
                {"neck", 1 },
                {"body", 1 },
                {"back", 1 },
                {"arms", 1 },
                {"hands", 1 },
                {"legs", 1 },
                {"feet", 1 },
                {"fingers", 1 },
            };

        private string xpMethod = DEFAULT_XP_CALCULATION_METHOD;
        private string goldMethod = DEFAULT_GOLD_CALCULATION_METHOD;

        private Dictionary<string, BigDouble> playerDefaultAttributes = new Dictionary<string, BigDouble>()
        {
            { Character.Attributes.ACCURACY, 10 },
            { Character.Attributes.ACTION_METER_SPEED, 0 },
            { Character.Attributes.CRITICAL_DAMAGE_MULTIPLIER, 1.2 },
            { Character.Attributes.CRITICAL_HIT_CHANCE, 10 },
            { Character.Attributes.DAMAGE, 10 },
            { Character.Attributes.DEFENSE, 10 },
            { Character.Attributes.EVASION, 10 },
            { Character.Attributes.MAXIMUM_HEALTH, 25 },
            { Character.Attributes.PENETRATION, 10 },
            { Character.Attributes.PRECISION, 10 },
            { Character.Attributes.RESILIENCE, 0 }
        };
        // These are the base values for these attributes for creatures.
        // These values are multiplied by the creature properties and then by the level scaling function.
        private Dictionary<string, BigDouble> creatureBaseAttributes = new Dictionary<string, BigDouble>()
        {
            { Character.Attributes.ACCURACY, 5 },
            { Character.Attributes.ACTION_METER_SPEED, 0 },
            { Character.Attributes.CRITICAL_DAMAGE_MULTIPLIER, 1.1 },
            { Character.Attributes.CRITICAL_HIT_CHANCE, 2 },
            { Character.Attributes.DAMAGE, 2 },
            { Character.Attributes.DEFENSE, 5 },
            { Character.Attributes.EVASION, 5 },
            { Character.Attributes.MAXIMUM_HEALTH, 10 },
            { Character.Attributes.PENETRATION, 5 },
            { Character.Attributes.PRECISION, 5 },
            { Character.Attributes.RESILIENCE, 0 }
        };

        public void ConfigureEngine(IdleEngine engine)
        {
            UserData.RegisterType<Character>();
            UserData.RegisterProxyType<ValueContainerScriptProxy, Character>(c => new ValueContainerScriptProxy(c));
            engine.RegisterMethod("CharacterUpdateMethod", (ctx, args) => {

                return UserData.Create(Character.UpdateMethod(engine, args[0].ToObject<ValueContainer>().AsCharacter(), args[1].ToObject<BigDouble>()));
                //return ;
            });
            engine.RegisterMethod("OnActionPhaseChanged", (ctx, args) => {
                OnActionPhaseChanged(engine, args[0].ToObject(), args[1].ToObject(), args[2].CastToString());
                return DynValue.Nil;
            });
            engine.RegisterMethod("OnEncounterEnded", (ctx, args) => {
                OnEncounterEnded(engine);
                return DynValue.Nil;
            });

            var actionPhase = engine.CreateProperty(Properties.ActionPhase, "");
            actionPhase.Subscribe("internal", ValueChangedEvent.EventName, "OnActionPhaseChanged(value, previous, reason)", true);
            engine.Subscribe("internal", EncounterEndedEvent.EventName, "OnEncounterEnded()", ephemeral: true);
            engine.CreateProperty("level", 0);
            engine.CreateProperty("configuration.action_meter_required_to_act", 2.5).SetEphemeral();
            engine.CreateProperty("configuration.xp_calculation_method", xpMethod).SetEphemeral();
            engine.CreateProperty("configuration.gold_calculation_method", goldMethod).SetEphemeral();
            SetDefaultAttributeConfiguration(engine);

            engine.GeneratePlayer();

            engine.Subscribe("rpgModule", CharacterDiedEvent.EventName, "OnCharacterDied()", ephemeral: true);
        }

        private void SetDefaultAttributeConfiguration(IdleEngine engine)
        {
            foreach (var attribute in playerDefaultAttributes)
            {
                engine.GetProperty(String.Join(".", "configuration.default_player_stats", attribute.Key), IdleEngine.GetOperationType.GET_OR_CREATE)
                    .Set(attribute.Value);
            }
            foreach (var attribute in creatureBaseAttributes)
            {
                engine.GetProperty(String.Join(".", "configuration.base_creature_stats", attribute.Key), IdleEngine.GetOperationType.GET_OR_CREATE)
                    .Set(attribute.Value);
            }
        }

        //public void 

        public void SetXpCalculationMethod(string method)
        {
            xpMethod = method;
        }

        [HandledEvent(typeof(CharacterDiedEvent))]
        public object OnCharacterDied(IdleEngine engine, params object[] args)
        {
            var killed = args[0] as Character;
            if (killed.Party == 0)
            {
                engine.SetActionPhase("");
            }
            else
            {
                Character player = engine.GetProperty("player").AsCharacter();
                player.Xp += engine.CalculateXpValue(args[0] as Character);

                player.Gold += engine.CalculateGoldValue(args[0] as Character);

                var currentEncounter = engine.GetCurrentEncounter();
                if (currentEncounter != null)
                {
                    var allCreaturesDead = currentEncounter["creatures"].ValueAsList().All(c => !c.AsCharacter().IsAlive);
                    if (allCreaturesDead)
                    {
                        engine.StartEncounter();
                    }
                }
            }
            return null;
        }

        public void AddCreature(CreatureDefinition creatureDefinition)
        {
            foreach (var property in new string[] { Character.Attributes.ACCURACY, Character.Attributes.EVASION, Character.Attributes.MAXIMUM_HEALTH })
            {
                AssertHasProperty(creatureDefinition, property);
            }
            definitions["creature"][creatureDefinition.Id] = creatureDefinition;
        }

        private void AssertHasProperty(CreatureDefinition creatureDefinition, string property)
        {
            if (!creatureDefinition.Properties.ContainsKey(property))
            {
                throw new InvalidOperationException(string.Format("Creature with id {0} is missing required property {1}", creatureDefinition.Id, property));
            }
        }

        public void AddEncounter(EncounterDefinition encounterDefinition)
        {
            foreach (var option in encounterDefinition.CreatureOptions)
            {
                if (!definitions["creature"].ContainsKey(option.Item1))
                {
                    throw new InvalidOperationException("Add a creature with id " + option.Item1 + " first!");
                }
            }
            if (encounterDefinition.CreatureOptions.Length == 0)
            {
                throw new InvalidOperationException("Encounter needs at least one option.");
            }
            definitions["encounter"][encounterDefinition.Id] = encounterDefinition;
        }

        public void AddItem(ItemDefinition item)
        {
            definitions["item"][item.Id] = item;
        }

        private IDictionary<string, IDictionary<string, IDefinition>> definitions = new Dictionary<string, IDictionary<string, IDefinition>>()
        {
            { "status", new Dictionary<string, IDefinition>() },
            { "creature", new Dictionary<string, IDefinition>() },
            { "encounter", new Dictionary<string, IDefinition>() },
            { "item", new Dictionary<string, IDefinition>() }
        };
        public Dictionary<string, int> ItemSlots { get; set; } = defaultItemSlots;

        public IDictionary<string, IDictionary<string, IDefinition>> GetDefinitions()
        {
            return definitions;
        }

        public void AddStatus(StatusDefinition statusDefinition)
        {
            definitions["status"].Add(statusDefinition.Id, statusDefinition);
        }

        public void AssertReady()
        {
            if (definitions["encounter"].Count == 0)
            {
                throw new InvalidOperationException("Need to define at least 1 encounter");
            }
        }

        private object OnEncounterEnded(IdleEngine engine, params object[] args)
        {
            OnCombatStarted(engine, args);
            return null;
        }

        [HandledEvent(typeof(ValueChangedEvent))]
        private void OnActionPhaseChanged(IdleEngine engine, object newValue, object oldValue, string reason)
        {
            if (object.Equals(oldValue, ""))
            {
                engine.GetPlayer().Reset();
            }
            else if (object.Equals(oldValue, "combat") && reason.ToString() != "restored")
            {
                OnCombatStarted(engine);
            }
        }

        private object OnCombatStarted(IdleEngine engine, params object[] args)
        {
            var player = engine.GetProperty("player");
            var playerMap = player.ValueAsMap();
            var playerAction = playerMap[Character.Attributes.ACTION];
            playerAction.Set(Character.Actions.FIGHT);
            engine.StartEncounter();
            return null;
        }

        public static class Properties
        {
            public const string ActionPhase = "action_phase";
        }
    }

    public static class EngineExtensionMethods
    {
        private static Dictionary<ValueContainer, Character> WrappedCharacterCache = new Dictionary<ValueContainer, Character>();
        public static AttackResultDescription MakeAttack(this IdleEngine engine, Character attacker, Character defender)
        {
            BigDouble attackDamage = engine.CalculateAttackDamage(attacker, defender);
            var context = new Dictionary<string, object>()
            {
                { "attacker", attacker },
                { "defender", defender }
            };
            int attackRoll = engine.RandomInt(100) + 1;

            BigDouble attackRollTarget = BigDouble.Max(0, BigDouble.Min(100, attacker.Accuracy - defender.Evasion + 50)); // FIXME: Set base to-hit change
            if (attackRoll <= attackRollTarget)
            {
                int criticalRoll = engine.RandomInt(1000) + 1;
                BigDouble criticalRollTarget = BigDouble.Max(0, BigDouble.Min(1000, attacker.Precision - defender.Resilience));
                string attackResult = "hit";
                if (criticalRoll <= criticalRollTarget)
                {
                    engine.Log(LogType.Log, () => String.Format("Character {0} critically hit character {1} with an attack: rolled {2}/{4} vs {3}/{5}", attacker.Id, defender.Id, attackRoll, attackRollTarget, criticalRoll, criticalRollTarget), "character.combat");
                    attackDamage = attackDamage * attacker.CriticalHitDamageMultiplier;

                    attackResult = "critical hit";
                } else
                {
                    engine.Log(LogType.Log, () => String.Format("Character {0} hit character {1} with an attack: rolled {2}/{4} vs {3}/{5}", attacker.Id, defender.Id, attackRoll, attackRollTarget, criticalRoll, criticalRollTarget), "character.combat");
                }
                ((ValueContainer)attacker).NotifyImmediately(AttackHitEvent.EventName, new AttackHitEvent(attacker, defender, attackDamage));
                ((ValueContainer)defender).NotifyImmediately(HitByAttackEvent.EventName, new HitByAttackEvent(attacker, defender, attackDamage));

                defender.InflictDamage(attackDamage, attacker);

                return new AttackResultDescription(true, attackResult);
            }
            else
            {
                engine.Log(LogType.Log, () => String.Format("Character {0} missed character {1} with an attack: rolled {2} vs {3}", attacker.Id, defender.Id, attackRoll, attackRollTarget), "character.combat");
                ((ValueContainer)attacker).NotifyImmediately(AttackMissEvent.EventName, new AttackMissEvent(attacker, defender));

                ((ValueContainer)defender).NotifyImmediately(MissedByAttackEvent.EventName, new MissedByAttackEvent(attacker, defender));
                return new AttackResultDescription(false, "miss");
            }
        }

        public static BigDouble CalculateXpValue(this IdleEngine engine, Character character)
        {
            return (BigDouble)engine.EvaluateExpression(engine.GetProperty("configuration.xp_calculation_method").ValueAsString(), new Dictionary<string, object>()
            {
                { "character", character }
            });
        }

        public static BigDouble CalculateGoldValue(this IdleEngine engine, Character character)
        {
            return (BigDouble)engine.EvaluateExpression(engine.GetProperty("configuration.gold_calculation_method").ValueAsString(), new Dictionary<string, object>()
            {
                { "character", character }
            });
        }

        public static IDictionary<string, ValueContainer> GetCurrentEncounter(this IdleEngine engine)
        {
            return engine.GetProperty("encounter").ValueAsMap();
        }

        public static string SetActionPhase(this IdleEngine engine, string actionPhase)
        {
            return engine.GetProperty(RpgModule.Properties.ActionPhase).Set(actionPhase);
        }

        public static BigDouble CalculateAttackDamage(this IdleEngine engine, Character attacker, Character defender)
        {
            BigDouble attackerDamage = attacker.Damage;

            return attackerDamage;
        }

        public static Character AsCharacter(this ValueContainer targetContainer)
        {
            Character character;
            if (!WrappedCharacterCache.TryGetValue(targetContainer, out character))
            {
                character = new Character(targetContainer);
                WrappedCharacterCache[targetContainer] = character;
            }
            return character;
        }

        public static void ApplyStatus(this IdleEngine engine, Character character, string statusId, float duration)
        {
            var statusDefinition = engine.GetDefinition<StatusDefinition>("status", statusId);
            if (statusDefinition == null)
            {
                throw new InvalidOperationException();
            }
            var characterStatuses = ((ValueContainer)character).GetProperty(Character.Attributes.STATUSES).ValueAsMap();
            if (!characterStatuses.ContainsKey(statusId))
            {
                ((ValueContainer)character).AddModifier(statusDefinition);
            }
            ((ValueContainer)character).GetProperty(Character.Attributes.STATUSES + "." + statusId, IdleEngine.GetOperationType.GET_OR_CREATE).Set(duration);
        }

        public static void RemoveStatus(this IdleEngine engine, Character character, string statusId)
        {
            var statusDefinition = engine.GetDefinition<StatusDefinition>("status", statusId);
            if (statusDefinition == null)
            {
                throw new InvalidOperationException();
            }
            var characterStatuses = ((ValueContainer)character).GetProperty(Character.Attributes.STATUSES).ValueAsMap();
            if (characterStatuses.ContainsKey(statusId))
            {
                ((ValueContainer)character).RemoveModifier(statusDefinition);
            }
            characterStatuses[statusId].Set((string)null);
        }

        private static EncounterDefinition GetRandomEncounter(this IdleEngine engine)
        {
            var encounters = engine.GetDefinitions<EncounterDefinition>("encounter");
            if (encounters != null)
            {
                var options = encounters.ToList();
                int index = engine.RandomInt(options.Count);
                return options[index];
            }
            throw new InvalidOperationException();
        }

        /*
         * Start a new encounter.
         * 
         * If an encounter definition is given, use that definition to generate the encounter. Otherwise, use a random one.
         */
        public static void StartEncounter(this IdleEngine engine, EncounterDefinition encounter = null)
        {
            var encounterCreatures = engine.CreateValueContainer(new List<ValueContainer>());
            engine.GetProperty("encounter", IdleEngine.GetOperationType.GET_OR_CREATE).Set(new Dictionary<string, ValueContainer>()
            {
                { "creatures", encounterCreatures }
            });
            if (encounter == null)
            {
                encounter = engine.GetRandomEncounter();
            }
            foreach (var option in encounter.CreatureOptions)
            {
                var creatureDefinition = engine.GetDefinition<CreatureDefinition>("creature", option.Item1);
                if (creatureDefinition == null)
                {
                    throw new InvalidOperationException();
                }
                var level = engine.GetProperty("level").ValueAsNumber() + option.Item2;
                var creature = engine.GenerateCreature(creatureDefinition, level);
                creature.GetProperty("id").Set(creature.Id);
                creature.GetProperty(Character.Attributes.ACTION).Set(Character.Actions.FIGHT);
                encounterCreatures.ValueAsList().Add(creature);
            }
            engine.NotifyImmediately(EncounterStartedEvent.EventName, null, new EncounterStartedEvent());
        }

        public static Character GetRandomTarget(this IdleEngine engine, BigDouble friendlyParty)
        {
            if (friendlyParty == 0)
            {
                var monsters = engine.GetProperty("encounter.creatures").ValueAsList();
                int index = engine.RandomInt(monsters.Count);
                return monsters[index].AsCharacter();
            }
            else
            {
                return engine.GetProperty("player").AsCharacter();
            }
        }

        public static Character GetPlayer(this IdleEngine engine)
        {
            return engine.GetProperty("player").AsCharacter();
        }

        private static Character GenerateCharacter(this IdleEngine engine)
        {
            var character = engine.CreateValueContainer(new Dictionary<string, ValueContainer>()).AsCharacter();
            character.CriticalHitChance = 0;
            character.Accuracy = 0;
            character.Evasion = 0;
            character.Defense = 0;
            character.Penetration = 0;
            character.Resilience = 0;
            return character;
        }

        public static Character GenerateCreature(this IdleEngine engine, CreatureDefinition creatureDefinition, BigDouble level)
        {
            var character = engine.GenerateCharacter();
            character.Accuracy = engine.GetProperty("configuration.base_creature_stats.accuracy").AsNumber * (BigDouble)engine.EvaluateExpression("return " + creatureDefinition.Properties[Character.Attributes.ACCURACY] as string);
            character.CriticalHitChance = engine.GetProperty("configuration.base_creature_stats.critical_hit_chance").AsNumber * (BigDouble)engine.EvaluateExpression("return " + creatureDefinition.Properties[Character.Attributes.CRITICAL_HIT_CHANCE] as string);
            character.CriticalHitDamageMultiplier = engine.GetProperty("configuration.base_creature_stats.critical_damage_multiplier").AsNumber * (BigDouble)engine.EvaluateExpression("return " + creatureDefinition.Properties[Character.Attributes.CRITICAL_DAMAGE_MULTIPLIER] as string);
            character.CurrentHealth = character.MaximumHealth = engine.GetProperty("configuration.base_creature_stats.maximum_health").AsNumber * (BigDouble)engine.EvaluateExpression("return " + creatureDefinition.Properties[Character.Attributes.MAXIMUM_HEALTH] as string);
            character.Damage = engine.GetProperty("configuration.base_creature_stats.damage").AsNumber * (BigDouble)engine.EvaluateExpression("return " + creatureDefinition.Properties[Character.Attributes.DAMAGE] as string);
            character.Defense = engine.GetProperty("configuration.base_creature_stats.defense").AsNumber * (BigDouble)engine.EvaluateExpression("return " + creatureDefinition.Properties[Character.Attributes.DEFENSE] as string);
            character.Evasion = engine.GetProperty("configuration.base_creature_stats.evasion").AsNumber * (BigDouble)engine.EvaluateExpression("return " + creatureDefinition.Properties[Character.Attributes.EVASION] as string);
            character.Penetration = engine.GetProperty("configuration.base_creature_stats.penetration").AsNumber * (BigDouble)engine.EvaluateExpression("return " + creatureDefinition.Properties[Character.Attributes.PENETRATION] as string);
            character.Precision = engine.GetProperty("configuration.base_creature_stats.precision").AsNumber * (BigDouble)engine.EvaluateExpression("return " + creatureDefinition.Properties[Character.Attributes.PRECISION] as string);
            character.Resilience = engine.GetProperty("configuration.base_creature_stats.resilience").AsNumber * (BigDouble)engine.EvaluateExpression("return " + creatureDefinition.Properties[Character.Attributes.RESILIENCE] as string);

            character.Icon = creatureDefinition.Properties[Character.Attributes.ICON] as string;
            character.Party = 1;
            return character;
        }

        public static Character GeneratePlayer(this IdleEngine engine)
        {
            var character = engine.GenerateCharacter();
            character.Party = 0;
            character.Accuracy = engine.GetProperty("configuration.default_player_stats.accuracy").AsNumber;
            character.CriticalHitChance = engine.GetProperty("configuration.default_player_stats.critical_hit_chance").AsNumber;
            character.CriticalHitDamageMultiplier = engine.GetProperty("configuration.default_player_stats.critical_damage_multiplier").AsNumber;
            character.CurrentHealth = character.MaximumHealth = engine.GetProperty("configuration.default_player_stats.maximum_health").AsNumber;
            character.Damage = engine.GetProperty("configuration.default_player_stats.damage").AsNumber;
            character.Defense = engine.GetProperty("configuration.default_player_stats.defense").AsNumber;
            character.Evasion = engine.GetProperty("configuration.default_player_stats.evasion").AsNumber;
            character.Penetration = engine.GetProperty("configuration.default_player_stats.penetration").AsNumber;
            character.Precision = engine.GetProperty("configuration.default_player_stats.precision").AsNumber;
            character.Resilience = engine.GetProperty("configuration.default_player_stats.resilience").AsNumber;

            character.Xp = 0;
            character.Gold = 0;

            character.GetProperty("known_powers", IdleEngine.GetOperationType.GET_OR_CREATE).Set(new Dictionary<string, ValueContainer>());

            engine.CreateProperty("player", (ValueContainer)character);
            return character;
        }

        public static bool InActiveEncounter(this IdleEngine engine)
        {
            var encounter = engine.GetProperty("encounter");
            return encounter != null && EncounterIsActive(encounter.ValueAsMap(), engine.GetProperty("player").ValueAsMap());
        }

        private static bool EncounterIsActive(IDictionary<string, ValueContainer> encounter, IDictionary<string, ValueContainer> player)
        {
            if (encounter == null)
            {
                return false;
            }
            var monsters = encounter["creatures"].ValueAsList();
            bool anyMonsterAlive = false;
            foreach (var monster in monsters)
            {
                anyMonsterAlive = anyMonsterAlive || !monster.AsCharacter().IsAlive;
            }
            bool playerAlive = player[Character.Attributes.CURRENT_HEALTH].ValueAsNumber() > 0;
            return anyMonsterAlive && playerAlive;
        }
    }
}