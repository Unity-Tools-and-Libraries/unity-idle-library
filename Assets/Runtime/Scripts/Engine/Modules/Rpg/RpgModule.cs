
using io.github.thisisnozaku.idle.framework.Events;
using MoonSharp.Interpreter;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg.Combat;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg.Events;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg.Configuration;

namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg
{
    public class RpgModule : IModule
    {
        private Dictionary<long, EncounterDefinition> encounters = new Dictionary<long, EncounterDefinition>();
        private Dictionary<long, CreatureDefinition> creatures = new Dictionary<long, CreatureDefinition>();
        private Dictionary<long, CharacterStatus> statuses = new Dictionary<long, CharacterStatus>();
        private Dictionary<long, CharacterAbility> abilities = new Dictionary<long, CharacterAbility>();
        private Dictionary<long, CharacterItem> items = new Dictionary<long, CharacterItem>();

        public CharacterConfiguration Player { get; } = new CharacterConfiguration();
        public CharacterConfiguration Creatures { get; } = new CharacterConfiguration();

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
           
        private string defaultAttackHitScript =
            "return {hit=true, description='hit', damageToTarget=attacker.damageValue - defender.damageReductionValue}";
        private string defaultAttackMissScript = "return {hit=false, description='miss', damageToTarget=0}";
        private string defaultAttackCriticalHitScript = "return {hit=true, description='critical hit', damageToTarget=(attacker.damageValue - defender.damageReductionValue) * attacker.criticalHitDamageMultiplier}";

        public void SetConfiguration(IdleEngine engine)
        {
            UserData.RegisterType<RpgCharacter>();
            UserData.RegisterType<RpgEncounter>();
            UserData.RegisterType<CreatureDefinition>();
            UserData.RegisterType<EncounterDefinition>();
            UserData.RegisterType<CharacterStatus>();
            UserData.RegisterType<AttackResultDescription>();
            UserData.RegisterType<CharacterItem>();
            UserData.RegisterType<CharacterAbility>();

            engine.SetConfiguration("PlayerType", Player.CharacterType);
            engine.SetConfiguration("action_meter_required_to_act", new BigDouble(2));
            engine.SetConfiguration("characterItemSlots", defaultItemSlots);
            engine.SetConfiguration("PlayerAttackCalculationScript", Player.AttackToHitScript);
            engine.SetConfiguration("PlayerAttackCalculationScript", Creatures.AttackToHitScript);
            engine.SetConfiguration("InitializePlayer", Player.PlayerInitializer);
        }

        public void SetDefinitions(IdleEngine engine)
        {
            engine.GetDefinitions()["encounters"] = encounters;
            engine.GetDefinitions()["creatures"] = creatures;
            engine.GetDefinitions()["statuses"] = statuses;
            engine.GetDefinitions()["abilities"] = abilities;
            engine.GetDefinitions()["items"] = items;
        }

        public void SetGlobalProperties(IdleEngine engine)
        {
            engine.GlobalProperties["actionPhase"] = "";
            engine.GlobalProperties["stage"] = new BigDouble(1);

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
                   engine.GetPlayer<RpgCharacter>().Xp += engine.Scripting.Evaluate("return calculateXpValue(creature)", new Dictionary<string, object>()
                   {
                        { "creature", DynValue.FromObject(null, creature).Clone(true) }
                   }).ToObject<BigDouble>();
                   engine.GetPlayer<RpgCharacter>().Gold += engine.Scripting.Evaluate("return calculateGoldValue(creature)", new Dictionary<string, object>()
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
            engine.GlobalProperties["InitializePlayer"] = Player.PlayerInitializer;


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

            engine.GlobalProperties["DetermineAttackValueScript"] = "return self.Accuracy";
            engine.GlobalProperties["DetermineDefenseValueScript"] = "return self.Evasion";
            engine.GlobalProperties["DetermineDamageValueScript"] = "return self.Damage";
            engine.GlobalProperties["DetermineDamageReductionValueScript"] = "return self.Defense";

            

            engine.GlobalProperties["AttackResultDescription"] = typeof(AttackResultDescription);

            engine.GlobalProperties["AttackHandlerScripts"] = new Dictionary<string, string>()
            {
                { "hit", defaultAttackHitScript },
                { "miss", defaultAttackMissScript },
                { "critical hit", defaultAttackCriticalHitScript }
            };

            SetDefaultAttributeConfiguration(engine);

            engine.GlobalProperties["startEncounter"] = (Func<EncounterDefinition, RpgEncounter>)engine.StartEncounter;

            engine.Scripting.SetScriptToClrCustomConversion(DataType.Table, typeof(AttackResultDescription), value =>
            {
                var table = value.Table;
                bool hit = (bool)table["hit"];
                string description = (string)table["description"];
                BigDouble damageToTarget = table.Get("damageToTarget").ToObject<BigDouble>();
                return new AttackResultDescription(hit, description, damageToTarget, 0, null, null); 
            });

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

        public void AddAbility(CharacterAbility ability)
        {
            abilities[ability.Id] = ability;
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

        public void AddItem(CharacterItem item)
        {
            items[item.Id] = item;
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
            var creature = new RpgCharacter(engine, engine.GetNextAvailableId());
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

        public RpgCharacter DefaultPlayerInitializer(IdleEngine engine)
        {
            return new RpgCharacter(engine, 0);
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

        public static Dictionary<long, CharacterItem> GetItems(this IdleEngine engine)
        {
            return (Dictionary<long, CharacterItem>)engine.GetDefinitions()["items"];
        }

        public static Dictionary<long, CharacterAbility> GetAbilities(this IdleEngine engine)
        {
            return (Dictionary<long, CharacterAbility>)engine.GetDefinitions()["abilities"];
        }

        public static void SetStage(this IdleEngine engine, BigDouble newStage)
        {
            engine.GlobalProperties["stage"] = newStage;
            engine.Emit(StageChangedEvent.EventName, new StageChangedEvent(newStage));
        }

        public static AttackResultDescription MakeAttack(this IdleEngine engine, RpgCharacter attacker, RpgCharacter defender)
        {
            var context = new Dictionary<string, object>()
            {
                { "attacker", attacker },
                { "defender", defender }
            };
            var attackResult = engine.Scripting.Evaluate(attacker.AttackScript, context).String;
            if(attackResult == null)
            {
                throw new InvalidOperationException("Value returned from AttackCalculationScript was not a string!");
            }
            
            Dictionary<string, string> resultHandlerScripts = engine.GlobalProperties["AttackHandlerScripts"] as Dictionary<string, string>;
            AttackResultDescription attackResultDescription;
            attackResultDescription = engine.Scripting.Evaluate(resultHandlerScripts[attackResult], context).ToObject<AttackResultDescription>();
            context["attack"] = attackResultDescription;
            // Call IsAttacking triggers on attacker
            List<string> IsAttackingTriggers;
            if (attacker.OnEventTriggers.TryGetValue("IsAttacking", out IsAttackingTriggers))
            {
                IsAttackingTriggers.Aggregate(attackResultDescription, (result, trigger) => engine.Scripting.Evaluate(trigger, context).ToObject<AttackResultDescription>());
            }
            // Call IsBeingAttacked triggers on defender
            List<string> IsBeingAttackedTriggers;
            if (defender.OnEventTriggers.TryGetValue("IsAttacking", out IsBeingAttackedTriggers))
            {
                IsBeingAttackedTriggers.Aggregate(attackResultDescription, (result, trigger) => engine.Scripting.Evaluate(trigger, context).ToObject<AttackResultDescription>());
            }
            if (attackResultDescription.IsHit)
            {
                attacker.Emit(AttackHitEvent.EventName, new AttackHitEvent(attacker, defender, attackResultDescription.DamageToDefender));
                defender.Emit(HitByAttackEvent.EventName, new HitByAttackEvent(attacker, defender, attackResultDescription.DamageToDefender));
            } else
            {
                attacker.Emit(AttackMissedEvent.EventName, new AttackMissedEvent(attacker, defender, attackResultDescription.DamageToDefender));
                defender.Emit(MissedByAttackEvent.EventName, new MissedByAttackEvent(attacker, defender, attackResultDescription.DamageToDefender));
            }

            return attackResultDescription;
        }

        public static RpgEncounter GetCurrentEncounter(this IdleEngine engine)
        {
            return engine.GlobalProperties["encounter"] as RpgEncounter;
        }

        public static void SetActionPhase(this IdleEngine engine, string actionPhase)
        {
            engine.GlobalProperties[RpgModule.Properties.ActionPhase] = actionPhase;
        }

        private static EncounterDefinition GetRandomEncounter(this IdleEngine engine)
        {
            var encounters = engine.GetEncounterDefinitions().Values;
            var options = encounters.ToList();
            int index = (int)engine.RandomInt(options.Count()).ToDouble();
            return options[index];
        }

        /*
         * Start a new encounter.
         * 
         * If an encounter definition is given, use that definition to generate the encounter. Otherwise, use a random one.
         */
        public static RpgEncounter StartEncounter(this IdleEngine engine, EncounterDefinition nextEncounter = null)
        {
            RpgEncounter currentEncounter = (RpgEncounter)(engine.GlobalProperties["encounter"] = DynValue.FromObject(null, new RpgEncounter(engine, engine.GetNextAvailableId())).ToObject<RpgEncounter>());
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
                int index = (int)engine.RandomInt(monsters.Count).ToDouble();
                return monsters[index];
            }
            else
            {
                return engine.GetPlayer<RpgCharacter>();
            }
        }

        public static T GetPlayer<T>(this IdleEngine engine) where T : RpgCharacter
        {
            return engine.GlobalProperties["player"] as T;
        }


        public static RpgCharacter GeneratePlayer(this IdleEngine engine)
        {
            var playerType = engine.GetConfiguration<Type>("PlayerType");
            if(!typeof(RpgCharacter).IsAssignableFrom(playerType))
            {
                throw new InvalidOperationException("PlayerType must be RpgCharacter or a subclass, but was " + playerType.ToString());
            }
            var player = Activator.CreateInstance(playerType, engine, 0);
            engine.Scripting.Evaluate(DynValue.FromObject(null, engine.GetConfiguration<string>("InitializePlayer")), new Dictionary<string, object>()
            {
                { "player", player }
            }).ToObject<RpgCharacter>();
            engine.GlobalProperties["player"] = player;
            return player as RpgCharacter;
        }
    }
}