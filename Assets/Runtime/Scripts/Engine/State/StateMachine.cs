using io.github.thisisnozaku.scripting.context;
using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;

namespace io.github.thisisnozaku.idle.framework.Engine.State
{
    public class StateMachine : CommandReceiver, IEventSource
    {
        public const string DEFAULT_STATE = "initial";
        private string currentState;

        private IDictionary<string, IDictionary<string, CommandHandler>> handlers = new Dictionary<string, IDictionary<string, CommandHandler>>();
        private IDictionary<string, IDictionary<string, TransitionConfiguration>> configuredTransitions = new Dictionary<string, IDictionary<string, TransitionConfiguration>>();
        private IDictionary<string, CommandHandler> builtInCommands = new Dictionary<string, CommandHandler>();
        private IdleEngine engine;
        private readonly EventListeners listeners;

        public StateMachine(IdleEngine engine)
        {
            this.currentState = DEFAULT_STATE;
            this.engine = engine;
            builtInCommands["transition"] = new CommandHandler((ie, args) =>
           {
               Transition(args[1]);
           }, "transition [destination state name]");
            listeners = new EventListeners(engine);
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

        public void EvaluateCommand(string command)
        {
            string[] tokens = command.Split(" ");
            CommandHandler handler;
            if (!builtInCommands.TryGetValue(tokens[0], out handler))
            {
                if (!(handlers.ContainsKey(currentState) && handlers[currentState].TryGetValue(tokens[0], out handler)))
                {
                    throw new InvalidOperationException(string.Format("Current state '{0}' cannot handle command '{1}'. {2}", currentState, command, GenerateHelpMessage()));
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

        public void Transition(string destination)
        {
            IDictionary<string, TransitionConfiguration> possibleTransitions;
            if (!configuredTransitions.TryGetValue(currentState, out possibleTransitions))
            {
                throw new InvalidOperationException(String.Format("No transitions are defined from state '{0}'.", currentState));
                
            }
            TransitionConfiguration transitionConfiguration;
            if(!possibleTransitions.TryGetValue(destination, out transitionConfiguration))
            {
                throw new InvalidOperationException(String.Format("No transition defined from '{0}' -> '{1}'.", currentState, destination));
            }
            var blockReason = transitionConfiguration.TransitionGuard(engine);
            if (blockReason != null)
            {
                throw new InvalidOperationException(string.Format("Blocked transition from '{0}' -> '{1}': {2}.", currentState, destination, blockReason));
            }
            var oldState = currentState;
            currentState = destination;
            Emit(StateChangedEvent.EventName, new StateChangedEvent(currentState, oldState));
        }

        public void Emit(string eventName, IScriptingContext contextToUse = null)
        {
            listeners.Emit(eventName, contextToUse);
        }

        public void Emit(string eventName, IDictionary<string, object> contextToUse)
        {
            listeners.Emit(eventName, contextToUse);
        }

        public void Emit(string eventName, Tuple<string, object> contextToUse)
        {
            listeners.Emit(eventName, contextToUse);
        }

        public void Watch(string eventName, string watcherIdentifier, string handler)
        {
            listeners.Watch(eventName, watcherIdentifier, handler);
        }

        public void Watch(string eventName, string subscriber, DynValue handler)
        {
            listeners.Watch(eventName, subscriber, handler);
        }

        public void StopWatching(string eventName, string subscriptionIdentifier)
        {
            listeners.StopWatching(eventName, subscriptionIdentifier);
        }

        private string GenerateHelpMessage()
        {
            string message = "";
            if (handlers.ContainsKey(currentState))
            {
                foreach (var command in handlers[currentState])
                {
                    message += Environment.NewLine + command.Key + " : " + command.Value.Describe();
                }
            }
            return message;
        }
    }
}