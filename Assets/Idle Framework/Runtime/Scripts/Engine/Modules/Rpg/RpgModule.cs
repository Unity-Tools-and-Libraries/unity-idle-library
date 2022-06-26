using io.github.thisisnozaku.idle.framework.Definitions;
using io.github.thisisnozaku.idle.framework.Events;
using MoonSharp.Interpreter;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using BreakInfinity;

namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg
{
    public class RpgModule : IModule
    {
        public const string DEFAULT_XP_CALCULATION_METHOD = "return 10 * pow(2, character.level.AsNumber - 1)";
        public const string DEFAULT_GOLD_CALCULATION_METHOD = "return 10 * pow(2, character.level.AsNumber - 1)";
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

        public void ConfigureEngine(IdleEngine engine)
        {
            UserData.RegisterType<Character>();
            UserData.RegisterProxyType<ValueContainerScriptProxy, Character>(c => new ValueContainerScriptProxy(c));
            engine.RegisterMethod("CharacterUpdateMethod", Character.UpdateMethod);
            engine.RegisterMethod(OnActionPhaseChanged);
            engine.RegisterMethod(OnEncounterEnded);

            engine.GeneratePlayer();
            var actionPhase = engine.CreateProperty(Properties.ActionPhase, "");
            actionPhase.Subscribe("internal", ValueChangedEvent.EventName, OnActionPhaseChanged, true);
            engine.Subscribe("internal", EncounterEndedEvent.EventName, OnEncounterEnded, ephemeral: true);
            engine.CreateProperty("level", 0);
            engine.CreateProperty("configuration.action_meter_required_to_act", 2.5).SetEphemeral();
            engine.CreateProperty("configuration.xp_calculation_method", xpMethod).SetEphemeral();
            engine.CreateProperty("configuration.gold_calculation_method", goldMethod).SetEphemeral();

            engine.Subscribe("rpgModule", CharacterDiedEvent.EventName, OnCharacterDied, ephemeral: true);
        }

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

        private object OnEncounterEnded(IdleEngine engine, object[] args)
        {
            OnCombatStarted(engine, args);
            return null;
        }

        [HandledEvent(typeof(ValueChangedEvent))]
        private object OnActionPhaseChanged(IdleEngine engine, params object[] args)
        {
            bool same = args[1].Equals(args[2]);
            if (args[2].ToString() == "")
            {
                engine.GetPlayer().Reset();
            } else if (args[2].Equals("combat") && args[3].ToString() != "restored")
            {
                OnCombatStarted(engine, args);
            }
            return args[1];
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
}