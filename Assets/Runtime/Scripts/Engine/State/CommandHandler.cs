using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace io.github.thisisnozaku.idle.framework.Engine.State
{
    public class CommandHandler
    {
        private Action<IdleEngine, string[]> handlerDelegate;
        private string usageMessage;
        private string description;

        public CommandHandler(Action<IdleEngine, string[]> handlerDelegate, string usageMessage, string description = "")
        {
            this.handlerDelegate = handlerDelegate;
            this.usageMessage = usageMessage;
            this.description = description;
        }

        public void Handle(IdleEngine engine, string[] args)
        {
            try
            {
                this.handlerDelegate(engine, args);
            } catch(Exception ex)
            {
                throw new InvalidOperationException(String.Format("{0} Usage: {1}", ex.Message, usageMessage));
            }
        }

        public string Describe()
        {
            return description;
        }
    }
}