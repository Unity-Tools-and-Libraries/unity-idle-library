using io.github.thisisnozaku.idle.framework.Definitions;
using io.github.thisisnozaku.idle.framework.Engine.Modules;
using io.github.thisisnozaku.idle.framework.Events;
using MoonSharp.Interpreter;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg
{
    public class RpgModule : IModule
    {
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
        private IDictionary<string, IDictionary<string, IDefinition>> definitions = new Dictionary<string, IDictionary<string, IDefinition>>()
        {
            { "status", new Dictionary<string, IDefinition>() }
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

        public void ConfigureEngine(IdleEngine engine)
        {
            UserData.RegisterType<Character>();
            UserData.RegisterProxyType<ValueContainerScriptProxy, Character>(c => new ValueContainerScriptProxy(c));
            engine.RegisterMethod("CharacterUpdateMethod", Character.UpdateMethod);
            engine.RegisterMethod(OnActionPhaseChanged);
            engine.RegisterMethod(OnEncounterEnded);

            engine.CreateProperty("player", ((ValueContainer)engine.GeneratePlayer()).ValueAsMap());
            var actionPhase = engine.CreateProperty(Properties.ActionPhase, "");
            actionPhase.Subscribe("internal", ValueChangedEvent.EventName, OnActionPhaseChanged);
            engine.Subscribe("internal", EncounterEndedEvent.EventName, OnEncounterEnded);
            engine.CreateProperty("level", 0);
            engine.CreateProperty("configuration.action_meter_required_to_act", 2.5);

        }

        private object OnEncounterEnded(IdleEngine engine, object[] args)
        {
            OnCombatStarted(engine, args);
            return null;
        }

        [HandledEvent(typeof(ValueChangedEvent))]
        private object OnActionPhaseChanged(IdleEngine engine, params object[] args)
        {
            if (engine.IsReady)
            {
                switch (args[2] as string)
                {
                    case "combat":
                        OnCombatStarted(engine, args);
                        break;
                }
            }
            return args[1];
        }

        private object OnCombatStarted(IdleEngine engine, params object[] args)
        {
            engine.GetProperty("player").ValueAsMap()[Character.Attributes.ACTION].Set(Character.Actions.FIGHT);
            engine.StartEncounter();
            return null;
        }

        public static class Properties
        {
            public const string ActionPhase = "action_phase";
        }
    }
}