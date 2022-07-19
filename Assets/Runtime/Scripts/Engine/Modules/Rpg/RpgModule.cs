
using io.github.thisisnozaku.idle.framework.Events;
using MoonSharp.Interpreter;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg.Combat;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg.Events;

namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg
{
    public class RpgModule : IModule
    {
        private Dictionary<long, EncounterDefinition> encounters = new Dictionary<long, EncounterDefinition>();
        private Dictionary<long, CreatureDefinition> creatures = new Dictionary<long, CreatureDefinition>();
        private Dictionary<long, CharacterStatus> statuses = new Dictionary<long, CharacterStatus>();
        private Dictionary<long, CharacterAbility> abilities = new Dictionary<long, CharacterAbility>();
        private Dictionary<long, RpgItem> items = new Dictionary<long, RpgItem>();

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
        private string AttackCalcuationScript =
            "local accuracy = attacker.AttackValue" + "\n" +
            "local evasion = defender.DefenseValue" + "\n" +
            "local toHitRoll = engine.RandomInt(100) + 1" + "\n" +
            "if toHitRoll <= (accuracy - evasion) then" + "\n" +
            "    local critRoll = engine.RandomInt(1000)" + "\n" +
            "    if critRoll <= attacker.CriticalHitChance then" + "\n" +
            "       return 'critical hit'" + "\n" +
            "    else return 'hit'" + "\n" +
            "    end" + "\n" +
            "else" + "\n" +
            "    return 'miss'" + "\n" +
            "end";
        private string defaultAttackHitScript =
            "return {hit=true, description='hit', damageToTarget=attacker.damageValue - defender.damageReductionValue}";
        private string defaultAttackMissScript = "return {hit=false, description='miss', damageToTarget=0}";
        private string defaultAttackCriticalHitScript = "return {hit=true, description='critical hit', damageToTarget=(attacker.damageValue - defender.damageReductionValue) * attacker.criticalHitDamageMultiplier}";

        public void ConfigureEngine(IdleEngine engine)
        {
            UserData.RegisterType<RpgCharacter>();
            UserData.RegisterType<RpgEncounter>();
            UserData.RegisterType<CreatureDefinition>();
            UserData.RegisterType<EncounterDefinition>();
            UserData.RegisterType<CharacterStatus>();
            UserData.RegisterType<AttackResultDescription>();

            engine.GlobalProperties["actionPhase"] = "";
            engine.GlobalProperties["stage"] = new BigDouble(1);
            engine.SetConfiguration("action_meter_required_to_act", new BigDouble(2));
            engine.SetConfiguration("characterItemSlots", defaultItemSlots);
            engine.SetConfiguration("AttackCalculationScript", AttackCalcuationScript);
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
                   engine.GetPlayer().Xp += engine.Scripting.EvaluateString("return calculateXpValue(creature)", new Dictionary<string, object>()
                   {
                        { "creature", DynValue.FromObject(null, creature).Clone(true) }
                   }).ToObject<BigDouble>();
                   engine.GetPlayer().Gold += engine.Scripting.EvaluateString("return calculateGoldValue(creature)", new Dictionary<string, object>()
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

            engine.GlobalProperties["calculateXpValue"] = (Func<RpgCharacter, BigDouble>)((character) => engine.Scripting.EvaluateString(engine.GetConfiguration<string>("xp_calculation_method"), new Dictionary<string, object>()
            {
                { "character", character }
            }).ToObject<BigDouble>());
            engine.GlobalProperties["calculateGoldValue"] = (Func<RpgCharacter, BigDouble>)((character) => engine.Scripting.EvaluateString(engine.GetConfiguration<string>("gold_calculation_script"), new Dictionary<string, object>()
            {
                { "character", character }
            }).ToObject<BigDouble>());

            engine.GlobalProperties["DetermineAttackValueScript"] = "return self.Accuracy";
            engine.GlobalProperties["DetermineDefenseValueScript"] = "return self.Evasion";
            engine.GlobalProperties["DetermineDamageValueScript"] = "return self.Damage";
            engine.GlobalProperties["DetermineDamageReductionValueScript"] = "return self.Defense";

            engine.GetDefinitions()["encounters"] = encounters;
            engine.GetDefinitions()["creatures"] = creatures;
            engine.GetDefinitions()["statuses"] = statuses;
            engine.GetDefinitions()["abilities"] = abilities;
            engine.GetDefinitions()["items"] = items;

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

        public void AddItem(RpgItem item)
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
            creature.Accuracy = engine.Scripting.EvaluateString(engine.GlobalProperties["ScaleCreatureAttributeScript"] as string, context).ToObject<BigDouble>();

            context["base"] = creatureDefinition.Properties[RpgCharacter.Attributes.ACTION_SPEED];
            creature.ActionMeterSpeed = engine.Scripting.EvaluateString(engine.GlobalProperties["ScaleCreatureAttributeScript"] as string, context).ToObject<BigDouble>();

            context["base"] = creatureDefinition.Properties[RpgCharacter.Attributes.CRITICAL_DAMAGE_MULTIPLIER];
            creature.CriticalHitDamageMultiplier = engine.Scripting.EvaluateString(engine.GlobalProperties["ScaleCreatureAttributeScript"] as string, context).ToObject<BigDouble>();

            context["base"] = creatureDefinition.Properties[RpgCharacter.Attributes.CRITICAL_HIT_CHANCE];
            creature.CriticalHitChance = engine.Scripting.EvaluateString(engine.GlobalProperties["ScaleCreatureAttributeScript"] as string, context).ToObject<BigDouble>();

            context["base"] = creatureDefinition.Properties[RpgCharacter.Attributes.MAXIMUM_HEALTH];
            creature.CurrentHealth = creature.MaximumHealth = engine.Scripting.EvaluateString(engine.GlobalProperties["ScaleCreatureAttributeScript"] as string, context).ToObject<BigDouble>();

            context["base"] = creatureDefinition.Properties[RpgCharacter.Attributes.PENETRATION];
            creature.Penetration = engine.Scripting.EvaluateString(engine.GlobalProperties["ScaleCreatureAttributeScript"] as string, context).ToObject<BigDouble>();

            context["base"] = creatureDefinition.Properties[RpgCharacter.Attributes.PRECISION];
            creature.Precision = engine.Scripting.EvaluateString(engine.GlobalProperties["ScaleCreatureAttributeScript"] as string, context).ToObject<BigDouble>();

            context["base"] = creatureDefinition.Properties[RpgCharacter.Attributes.PENETRATION];
            creature.Penetration = engine.Scripting.EvaluateString(engine.GlobalProperties["ScaleCreatureAttributeScript"] as string, context).ToObject<BigDouble>();

            context["base"] = creatureDefinition.Properties[RpgCharacter.Attributes.RESILIENCE];
            creature.Resilience = engine.Scripting.EvaluateString(engine.GlobalProperties["ScaleCreatureAttributeScript"] as string, context).ToObject<BigDouble>();

            creature.Level = level;

            creature.Watch(CharacterDiedEvent.EventName, "creature death handler", "on_creature_died(died)");

            return creature;
        }

        public void SetAttackCalcuationScript(string script)
        {
            AttackCalcuationScript = script;
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

        public static Dictionary<long, RpgItem> GetItems(this IdleEngine engine)
        {
            return (Dictionary<long, RpgItem>)engine.GetDefinitions()["items"];
        }

        public static Dictionary<long, CharacterAbility> GetAbilities(this IdleEngine engine)
        {
            return (Dictionary<long, CharacterAbility>)engine.GetDefinitions()["abilities"];
        }

        public static AttackResultDescription MakeAttack(this IdleEngine engine, RpgCharacter attacker, RpgCharacter defender)
        {
            var context = new Dictionary<string, object>()
            {
                { "attacker", attacker },
                { "defender", defender }
            };
            string script = engine.GetConfiguration<string>("AttackCalculationScript");
            var attackResult = engine.Scripting.EvaluateString(script, context).String;
            if(attackResult == null)
            {
                throw new InvalidOperationException("Value returned from AttackCalculationScript was not a string!");
            }
            
            Dictionary<string, string> resultHandlerScripts = engine.GlobalProperties["AttackHandlerScripts"] as Dictionary<string, string>;
            AttackResultDescription attackResultDescription;
            attackResultDescription = engine.Scripting.EvaluateString(resultHandlerScripts[attackResult], context).ToObject<AttackResultDescription>();
            context["attack"] = attackResultDescription;
            // Call IsAttacking triggers on attacker
            List<string> IsAttackingTriggers;
            if (attacker.OnEventTriggers.TryGetValue("IsAttacking", out IsAttackingTriggers))
            {
                IsAttackingTriggers.Aggregate(attackResultDescription, (result, trigger) => engine.Scripting.EvaluateString(trigger, context).ToObject<AttackResultDescription>());
            }
            // Call IsBeingAttacked triggers on defender
            List<string> IsBeingAttackedTriggers;
            if (defender.OnEventTriggers.TryGetValue("IsAttacking", out IsBeingAttackedTriggers))
            {
                IsBeingAttackedTriggers.Aggregate(attackResultDescription, (result, trigger) => engine.Scripting.EvaluateString(trigger, context).ToObject<AttackResultDescription>());
            }
            if (attackResultDescription.IsHit)
            {
                attacker.Emit(AttackHitEvent.EventName, new AttackHitEvent(attacker, defender, attackResultDescription.DamageToDefender));
                defender.Emit(HitByAttackEvent.EventName, new HitByAttackEvent(attacker, defender, attackResultDescription.DamageToDefender));
            } else
            {
                attacker.Emit(AttackFailedEvent.EventName, new AttackFailedEvent(attacker, defender, attackResultDescription.DamageToDefender));
                defender.Emit(MissedByAttack.EventName, new MissedByAttack(attacker, defender, attackResultDescription.DamageToDefender));
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
                var creature = engine.Scripting.EvaluateString("return GenerateCreature(definition, level)", new Dictionary<string, object>()
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
                return engine.GetPlayer();
            }
        }

        public static RpgCharacter GetPlayer(this IdleEngine engine)
        {
            return engine.GlobalProperties["player"] as RpgCharacter;
        }

        public static RpgCharacter GeneratePlayer(this IdleEngine engine)
        {
            var player = new RpgCharacter(engine, engine.GetNextAvailableId());
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