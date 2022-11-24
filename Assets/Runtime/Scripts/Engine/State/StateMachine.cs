using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace io.github.thisisnozaku.idle.framework.Engine.State
{
    public class StateMachine : CommandReceiver
    {
        public const string DEFAULT_STATE = "initial";
        private string currentState;

        private IDictionary<string, IDictionary<string, CommandHandler>> handlers = new Dictionary<string, IDictionary<string, CommandHandler>>();
        private IDictionary<string, IDictionary<string, TransitionConfiguration>> configuredTransitions = new Dictionary<string, IDictionary<string, TransitionConfiguration>>();
        private IDictionary<string, CommandHandler> builtInCommands = new Dictionary<string, CommandHandler>();

        public StateMachine()
        {
            this.currentState = DEFAULT_STATE;
            builtInCommands["transition"] = new CommandHandler((ie, args) =>
           {
               Transition(args[1], ie);
           }, "transition [destination state name]");
        }

        public string StateName => currentState;

        public void AddHandler(string stateName, string commandName, Action<IdleEngine, string[]> handler, string usageMessage)
        {
            IDictionary<string,CommandHandler> handlers;
            if(!this.handlers.TryGetValue(stateName, out handlers))
            {
                handlers = this.handlers[stateName] = new Dictionary<string, CommandHandler>();
            }
            handlers[commandName] = new CommandHandler(handler, usageMessage);
        }

        public void EvaluateCommand(string command, IdleEngine engine)
        {
            string[] tokens = command.Split(" ");
            CommandHandler handler;
            if (!builtInCommands.TryGetValue(tokens[0], out handler))
            {
                if (!(handlers.ContainsKey(currentState) && handlers[currentState].TryGetValue(tokens[0], out handler)))
                {
                    throw new InvalidOperationException(string.Format("Current state '{0}' cannot handle command '{1}'", currentState, command));
                }
            }
            handler.Handle(engine, tokens);

        }

        public void DefineTransition(string origin, string destination, Func<IdleEngine, string> guard = null)
        {
            IDictionary<string, TransitionConfiguration> possibleTransitions;
            if (!configuredTransitions.TryGetValue(origin, out possibleTransitions))
            {
                configuredTransitions[origin] = possibleTransitions = new Dictionary<string, TransitionConfiguration>();
            }
            possibleTransitions[destination] = new TransitionConfiguration(guard);
        }

        private class TransitionConfiguration
        {
            public Func<IdleEngine, string> TransitionGuard { get; }

            public TransitionConfiguration(Func<IdleEngine, string> transitionGuard)
            {
                if (transitionGuard != null)
                {
                    this.TransitionGuard = transitionGuard;
                } else
                {
                    this.TransitionGuard = ie => null;
                }
            }
        }

        public void Transition(string destination, IdleEngine engine)
        {
            IDictionary<string, TransitionConfiguration> possibleTransitions;
            if (!configuredTransitions.TryGetValue(currentState, out possibleTransitions))
            {
                throw new InvalidOperationException(String.Format("Cannot transitition from unknown state {0}.", currentState));
                
            }
            TransitionConfiguration transitionConfiguration;
            if(!possibleTransitions.TryGetValue(destination, out transitionConfiguration))
            {
                throw new InvalidOperationException(String.Format("No transition defined from {0} -> {1}", currentState, destination));
            }
            var blockReason = transitionConfiguration.TransitionGuard(engine);
            if (blockReason != null)
            {
                throw new InvalidOperationException(string.Format("Failed to transition from {0} -> {1}: {2}", currentState, destination, blockReason));
            }
            currentState = destination;
        }
    }
}