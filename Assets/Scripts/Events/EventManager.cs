using IdleFramework.UI.Events.Payloads;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework.Events
{
    public class EventManager
    {
        private readonly Dictionary<string, IList<Action<object>>> listeners = new Dictionary<string, IList<Action<object>>>();
        private readonly GameObject gameObject;

        public EventManager(GameObject gameObject)
        {
            this.gameObject = gameObject;
        }

        public void DispatchEvent(OpenPopupPayload payload)
        {
            DispatchEvent("openPopup", payload);
        }

        public void DispatchEvent(string eventName)
        {
            DispatchEvent(eventName, null);
        }

        public void DispatchEvent(string eventName, object payload)
        {
            IList<Action<object>> eventListeners;
            if(listeners.TryGetValue(eventName, out eventListeners))
            {
                foreach(var listener in eventListeners)
                {
                    listener.Invoke(payload);
                }
            }
            gameObject.BroadcastMessage(eventName, payload, SendMessageOptions.DontRequireReceiver);
        }

        public void AddListener(string eventName, Action<object> listener)
        {
            validateEventNameAndListenerArgument(eventName, listener);
            IList<Action<object>> eventListeners;
            if(!listeners.TryGetValue(eventName, out eventListeners)) {
                eventListeners = new List<Action<object>>();
                listeners.Add(eventName, eventListeners);
            }
            eventListeners.Add(listener);
        }

        private void validateEventNameAndListenerArgument(string name, Action<object> listener)
        {
            var genericArgument = listener.GetType().GetGenericArguments()[0];
            switch(name)
            {
                case "openPopup":
                    if(!genericArgument.Equals(typeof(OpenPopupPayload)))
                    {
                        throw new InvalidOperationException(string.Format("The type of the payload for the listener for {0} is invalid.", name));
                    }
                    break;
            }
        }
    }
}