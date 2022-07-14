
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
        private Dictionary<long, EncounterDefinition> encounters = new Dictionary<long, EncounterDefinition>();
        private Dictionary<long, CreatureDefinition> creatures = new Dictionary<long, CreatureDefinition>();
        private Dictionary<long, CharacterStatus> statuses = new Dictionary<long, CharacterStatus>();

        public const string DEFAULT_XP_CALCULATION_METHOD = "return 10 * math.pow(2, character.level - 1)";
        public const string DEFAULT_gold_calculation_script = "return 10 * math.pow(2, character.level - 1)";
        public const string DEFAULT_MONSTER_SCALING_FUNCTION = "return base * math.pow(1.1, level - 1)";
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

        private Dictionary<string, BigDouble> playerDefaultAttributes = new Dictionary<string, BigDouble>()
        {
            { RpgCharacter.Attributes.ACCURACY, 10 },
            { RpgCharacter.Attributes.ACTION_SPEED, 0 },
            { RpgCharacter.Attributes.CRITICAL_DAMAGE_MULTIPLIER, 1.2 },
            { RpgCharacter.Attributes.CRITICAL_HIT_CHANCE, 10 },
            { RpgCharacter.Attributes.DAMAGE, 10 },
            { RpgCharacter.Attributes.DEFENSE, 10 },
            { RpgCharacter.Attributes.EVASION, 10 },
            { RpgCharacter.Attributes.MAXIMUM_HEALTH, 25 },
            { RpgCharacter.Attributes.PENETRATION, 10 },
            { RpgCharacter.Attributes.PRECISION, 10 },
            { RpgCharacter.Attributes.RESILIENCE, 0 }
        };
        // These are the base values for these attributes for creatures.
        // These values are multiplied by the creature properties and then by the level scaling function.
        private Dictionary<string, BigDouble> creatureBaseAttributes = new Dictionary<string, BigDouble>()
        {
            { RpgCharacter.Attributes.ACCURACY, 5 },
            { RpgCharacter.Attributes.ACTION_SPEED, 0 },
            { RpgCharacter.Attributes.CRITICAL_DAMAGE_MULTIPLIER, 1.1 },
            { RpgCharacter.Attributes.CRITICAL_HIT_CHANCE, 2 },
            { RpgCharacter.Attributes.DAMAGE, 2 },
            { RpgCharacter.Attributes.DEFENSE, 5 },
            { RpgCharacter.Attributes.EVASION, 5 },
            { RpgCharacter.Attributes.MAXIMUM_HEALTH, 10 },
            { RpgCharacter.Attributes.PENETRATION, 5 },
            { RpgCharacter.Attributes.PRECISION, 5 },
            { RpgCharacter.Attributes.RESILIENCE, 0 }
        };

        public void ConfigureEngine(IdleEngine engine)
        {
            UserData.RegisterType<RpgCharacter>();
            UserData.RegisterType<RpgEncounter>();
            UserData.RegisterType<CreatureDefinition>();

            engine.GlobalProperties["actionPhase"] = "";
            engine.GlobalProperties["stage"] = new BigDouble(1);
            engine.SetConfiguration("action_meter_required_to_act", new BigDouble(2));
            engine.SetConfiguration("characterItemSlots", defaultItemSlots);
            if (engine.GetConfiguration<string>("xp_calculation_method") == null)
            {
                engine.SetConfiguration("xp_calculation_method", DEFAULT_XP_CALCULATION_METHOD);
            }
            if (engine.GetConfiguration<string>("gold_calculation_script") == null)
            {
                engine.SetConfiguration("gold_calculation_script", DEFAULT_gold_calculation_script);
            }
            if (engine.GetProperty<string>("on_creature_died") == null)
            {
                engine.GlobalProperties["on_creature_died"] = (Action<RpgCharacter>)(creature =>
               {
                   engine.GetPlayer().Xp += engine.Scripting.Evaluate("return calculateXpValue(creature)", new Dictionary<string, object>()
                   {
                        { "creature", DynValue.FromObject(null, creature).Clone(true) }
                   }).ToObject<BigDouble>();
                   engine.GetPlayer().Gold += engine.Scripting.Evaluate("return calculateGoldValue(creature)", new Dictionary<string, object>()
                   {
                        { "creature", DynValue.FromObject(null, creature).Clone(true) }
                   }).ToObject<BigDouble>();
                   if(!engine.GetCurrentEncounter().IsActive)
                   {
                       engine.Emit(EncounterEndedEvent.EventName, (ScriptingContext)null);
                   }
               });
            }
            if(engine.GetProperty("GenerateCreature") == null)
            {
                engine.GlobalProperties["GenerateCreature"] = (Func<CreatureDefinition, BigDouble, RpgCharacter>)((creature, level) => DefaultCreatureGenerator(engine, creature, level));
            }
            if(engine.GetProperty("ScaleCreatureAttributeScript") == null)
            {
                engine.GlobalProperties["ScaleCreatureAttributeScript"] = DEFAULT_MONSTER_SCALING_FUNCTION;
            }

            engine.GlobalProperties["calculateXpValue"] = (Func<RpgCharacter, BigDouble>)((character) => engine.Scripting.Evaluate(engine.GetConfiguration<string>("xp_calculation_method"), new Dictionary<string, object>()
            {
                { "character", character }
            }).ToObject<BigDouble>());
            engine.GlobalProperties["calculateGoldValue"] = (Func<RpgCharacter, BigDouble>)((character) => engine.Scripting.Evaluate(engine.GetConfiguration<string>("gold_calculation_script"), new Dictionary<string, object>()
            {
                { "character", character }
            }).ToObject<BigDouble>());

            engine.GetDefinitions()["encounters"] = encounters;
            engine.GetDefinitions()["creatures"] = creatures;
            engine.GetDefinitions()["statuses"] = statuses;


            SetDefaultAttributeConfiguration(engine);

            engine.GlobalProperties["startEncounter"] = (Func<EncounterDefinition, RpgEncounter>)engine.StartEncounter;

            engine.GeneratePlayer();
        }

        private void SetDefaultAttributeConfiguration(IdleEngine engine)
        {
            Dictionary<string, object> playerAttributes = (Dictionary<string, object>)(engine.GetConfiguration()["default_player_stats"] = new Dictionary<string, object>());
            Dictionary<string, object> creatureAttributes = (Dictionary<string, object>)(engine.GetConfiguration()["base_creature_stats"] = new Dictionary<string, object>());
            foreach (var attribute in playerDefaultAttributes)
            {
                playerAttributes[attribute.Key] = attribute.Value;
            }
            foreach (var attribute in creatureBaseAttributes)
            {
                creatureAttributes[attribute.Key] = attribute.Value;
            }
        }

        public void AddCreature(CreatureDefinition creatureDefinition)
        {
            creatures[creatureDefinition.Id] = creatureDefinition;
        }

        public void AddEncounter(EncounterDefinition encounterDefinition)
        {
            foreach (var option in encounterDefinition.CreatureOptions)
            {
                if (!creatures.ContainsKey(option.Item1))
                {
                    throw new InvalidOperationException("Add a creature with id " + option.Item1 + " first!");
                }
            }
            if (encounterDefinition.CreatureOptions.Length == 0)
            {
                throw new InvalidOperationException("Encounter needs at least one option.");
            }
            encounters[encounterDefinition.Id] = encounterDefinition;
        }

        public void AddStatus(CharacterStatus status)
        {
            statuses[status.Id] = status;
        }

        public Dictionary<string, int> ItemSlots { get; set; } = defaultItemSlots;

        public void AssertReady()
        {
            if (encounters.Count == 0)
            {
                throw new InvalidOperationException("Need to define at least 1 encounter");
            }
            if (creatures.Count == 0)
            {
                throw new InvalidOperationException("Need to define at least 1 creature");
            }
        }

        public RpgCharacter DefaultCreatureGenerator(IdleEngine engine, CreatureDefinition creatureDefinition, BigDouble level)
        {
            var creature = new RpgCharacter(engine);
            var context = new Dictionary<string, object>()
            {
                { "level", level }
            };
            creature.Party = 1;
            context["base"] = creatureDefinition.Properties[RpgCharacter.Attributes.ACCURACY];
            creature.Accuracy = engine.Scripting.Evaluate(engine.GlobalProperties["ScaleCreatureAttributeScript"] as string, context).ToObject<BigDouble>();

            context["base"] = creatureDefinition.Properties[RpgCharacter.Attributes.ACTION_SPEED];
            creature.ActionMeterSpeed = engine.Scripting.Evaluate(engine.GlobalProperties["ScaleCreatureAttributeScript"] as string, context).ToObject<BigDouble>();

            context["base"] = creatureDefinition.Properties[RpgCharacter.Attributes.CRITICAL_DAMAGE_MULTIPLIER];
            creature.CriticalHitDamageMultiplier = engine.Scripting.Evaluate(engine.GlobalProperties["ScaleCreatureAttributeScript"] as string, context).ToObject<BigDouble>();

            context["base"] = creatureDefinition.Properties[RpgCharacter.Attributes.CRITICAL_HIT_CHANCE];
            creature.CriticalHitChance = engine.Scripting.Evaluate(engine.GlobalProperties["ScaleCreatureAttributeScript"] as string, context).ToObject<BigDouble>();

            context["base"] = creatureDefinition.Properties[RpgCharacter.Attributes.MAXIMUM_HEALTH];
            creature.CurrentHealth = creature.MaximumHealth = engine.Scripting.Evaluate(engine.GlobalProperties["ScaleCreatureAttributeScript"] as string, context).ToObject<BigDouble>();

            context["base"] = creatureDefinition.Properties[RpgCharacter.Attributes.PENETRATION];
            creature.Penetration = engine.Scripting.Evaluate(engine.GlobalProperties["ScaleCreatureAttributeScript"] as string, context).ToObject<BigDouble>();

            context["base"] = creatureDefinition.Properties[RpgCharacter.Attributes.PRECISION];
            creature.Precision = engine.Scripting.Evaluate(engine.GlobalProperties["ScaleCreatureAttributeScript"] as string, context).ToObject<BigDouble>();

            context["base"] = creatureDefinition.Properties[RpgCharacter.Attributes.PENETRATION];
            creature.Penetration = engine.Scripting.Evaluate(engine.GlobalProperties["ScaleCreatureAttributeScript"] as string, context).ToObject<BigDouble>();

            context["base"] = creatureDefinition.Properties[RpgCharacter.Attributes.RESILIENCE];
            creature.Resilience = engine.Scripting.Evaluate(engine.GlobalProperties["ScaleCreatureAttributeScript"] as string, context).ToObject<BigDouble>();

            creature.Level = level;

            creature.Watch(CharacterDiedEvent.EventName, "creature death handler", "on_creature_died(died)");

            return creature;
            
        }

        public static class Properties
        {
            public const string ActionPhase = "action_phase";
        }
    }

    public static class RpgExtensionMethods
    {
        public static IDictionary<long, EncounterDefinition> GetEncounterDefinitions(this IdleEngine engine)
        {
            return engine.GetProperty<IDictionary<long, EncounterDefinition>>("definitions.encounters");
        }

        public static Dictionary<long, CharacterStatus> GetStatuses(this IdleEngine engine)
        {
            return (Dictionary<long, CharacterStatus>)engine.GetDefinitions()["statuses"];
        }

        public static Dictionary<long, CreatureDefinition> GetCreatures(this IdleEngine engine)
        {
            return (Dictionary<long, CreatureDefinition>)engine.GetDefinitions()["creatures"];
        }

        public static AttackResultDescription MakeAttack(this IdleEngine engine, RpgCharacter attacker, RpgCharacter defender)
        {
            BigDouble attackDamage = engine.CalculateAttackDamage(attacker, defender); // FIXME: Into configurable lua script
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
                    //engine.Log(LogType.Log, () => String.Format("Character {0} critically hit character {1} with an attack: rolled {2}/{4} vs {3}/{5}", attacker.Id, defender.Id, attackRoll, attackRollTarget, criticalRoll, criticalRollTarget), "character.combat");
                    attackDamage = attackDamage * attacker.CriticalHitDamageMultiplier;

                    attackResult = "critical hit";
                }
                else
                {
                    //engine.Log(LogType.Log, () => String.Format("Character {0} hit character {1} with an attack: rolled {2}/{4} vs {3}/{5}", attacker.Id, defender.Id, attackRoll, attackRollTarget, criticalRoll, criticalRollTarget), "character.combat");
                }
                //((PropertyMetadata)attacker).NotifyImmediately(AttackHitEvent.EventName, new AttackHitEvent(attacker, defender, attackDamage));
                //((PropertyMetadata)defender).NotifyImmediately(HitByAttackEvent.EventName, new HitByAttackEvent(attacker, defender, attackDamage));

                defender.InflictDamage(attackDamage, attacker);

                return new AttackResultDescription(true, attackResult);
            }
            else
            {
                //engine.Log(LogType.Log, () => String.Format("Character {0} missed character {1} with an attack: rolled {2} vs {3}", attacker.Id, defender.Id, attackRoll, attackRollTarget), "character.combat");
                //((PropertyMetadata)attacker).NotifyImmediately(AttackMissEvent.EventName, new AttackMissEvent(attacker, defender));

                //((PropertyMetadata)defender).NotifyImmediately(MissedByAttackEvent.EventName, new MissedByAttackEvent(attacker, defender));
                return new AttackResultDescription(false, "miss");
            }
        }

        public static RpgEncounter GetCurrentEncounter(this IdleEngine engine)
        {
            return engine.GlobalProperties["encounter"] as RpgEncounter;
        }

        public static void SetActionPhase(this IdleEngine engine, string actionPhase)
        {
            engine.GlobalProperties[RpgModule.Properties.ActionPhase] = actionPhase;
        }

        public static BigDouble CalculateAttackDamage(this IdleEngine engine, RpgCharacter attacker, RpgCharacter defender)
        {
            BigDouble attackerDamage = attacker.Damage;

            return attackerDamage;
        }

        private static EncounterDefinition GetRandomEncounter(this IdleEngine engine)
        {
            var encounters = engine.GetEncounterDefinitions().Values;
            var options = encounters.ToList();
            int index = engine.RandomInt(options.Count());
            return options[index];
        }

        /*
         * Start a new encounter.
         * 
         * If an encounter definition is given, use that definition to generate the encounter. Otherwise, use a random one.
         */
        public static RpgEncounter StartEncounter(this IdleEngine engine, EncounterDefinition nextEncounter = null)
        {
            RpgEncounter currentEncounter = (RpgEncounter)(engine.GlobalProperties["encounter"] = DynValue.FromObject(null, new RpgEncounter(engine)).ToObject<RpgEncounter>());
            if (nextEncounter == null)
            {
                nextEncounter = engine.GetRandomEncounter();
            }
            foreach (var option in nextEncounter.CreatureOptions)
            {
                var creatureDefinition = engine.GetCreatures()[option.Item1];
                if (creatureDefinition == null)
                {
                    throw new InvalidOperationException();
                }
                var level = (BigDouble)engine.GlobalProperties["stage"] + option.Item2;
                var creature = engine.Scripting.Evaluate("return GenerateCreature(definition, level)", new Dictionary<string, object>()
                {
                    { "definition", creatureDefinition },
                    { "level", level }
                }).ToObject<RpgCharacter>();

                currentEncounter.Creatures.Add(creature);
            }
            engine.Emit(EncounterStartedEvent.EventName, new EncounterStartedEvent());
            return currentEncounter;
        }

        public static RpgCharacter GetRandomTarget(this IdleEngine engine, BigDouble friendlyParty)
        {
            if (friendlyParty == 0)
            {
                var monsters = engine.GetCurrentEncounter().Creatures;
                int index = engine.RandomInt(monsters.Count);
                return monsters[index];
            }
            else
            {
                return engine.GetPlayer();
            }
        }

        public static RpgCharacter GetPlayer(this IdleEngine engine)
        {
            return engine.GlobalProperties["player"] as RpgCharacter;
        }

        public static RpgCharacter GeneratePlayer(this IdleEngine engine)
        {
            var player = new RpgCharacter(engine);
            player.Party = 0;
            player.Accuracy = engine.GetProperty<BigDouble>("configuration.default_player_stats.accuracy");
            player.CriticalHitChance = engine.GetProperty<BigDouble>("configuration.default_player_stats.critical_hit_chance");
            player.CriticalHitDamageMultiplier = engine.GetProperty<BigDouble>("configuration.default_player_stats.critical_damage_multiplier");
            player.CurrentHealth = player.MaximumHealth = engine.GetProperty<BigDouble>("configuration.default_player_stats.maximum_health");
            player.Damage = engine.GetProperty<BigDouble>("configuration.default_player_stats.damage");
            player.Defense = engine.GetProperty<BigDouble>("configuration.default_player_stats.defense");
            player.Evasion = engine.GetProperty<BigDouble>("configuration.default_player_stats.evasion");
            player.Penetration = engine.GetProperty<BigDouble>("configuration.default_player_stats.penetration");
            player.Precision = engine.GetProperty<BigDouble>("configuration.default_player_stats.precision");
            player.Resilience = engine.GetProperty<BigDouble>("configuration.default_player_stats.resilience");

            player.Xp = 0;
            player.Gold = 0;

            engine.GlobalProperties["player"] = player;

            player.Watch(CharacterDiedEvent.EventName, "begin resurrecting", "died.action = 'REINCARNATING'");

            return player;
        }
    }
}