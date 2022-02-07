using BreakInfinity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework
{
    /*
     * A Dictionary where updates in contained values
     */
    public class ParentNotifyingDictionary : IDictionary<string, ValueContainer>, Watchable
    {
        private IdleEngine engine;
        private Dictionary<string, ValueContainer> underlying = new Dictionary<string, ValueContainer>();
        private List<Action<object>> listeners = new List<Action<object>>();

        public ParentNotifyingDictionary(IdleEngine engine): this(engine, null)
        {
            
        }

        public ParentNotifyingDictionary(IdleEngine engine, IDictionary<string, ValueContainer> other)
        {
            if (other != null)
            {
                underlying = new Dictionary<string, ValueContainer>(other);
            } else
            {
                underlying = new Dictionary<string, ValueContainer>();
            }
            this.engine = engine;
        }
        public ValueContainer this[string key]
        {
            get
            {
                ValueContainer existing;
                if(!underlying.TryGetValue(key, out existing))
                {
                    existing = new ValueContainer();
                    engine.RegisterReference(existing);
                    underlying[key] = existing;
                    existing.Watch(x => NotifyListeners());
                }
                return existing;
            }
            set {
                underlying[key] = value;
                value.Watch(x => NotifyListeners());
            }
        }

        public ICollection<string> Keys => underlying.Keys;

        public ICollection<ValueContainer> Values => underlying.Values;

        public int Count => underlying.Count;

        public bool IsReadOnly => false;

        public void Add(string key, ValueContainer value)
        {
            throw new System.NotImplementedException();
        }

        public void Add(KeyValuePair<string, ValueContainer> item)
        {
            throw new System.NotImplementedException();
        }

        public void Clear()
        {
            throw new System.NotImplementedException();
        }

        public bool Contains(KeyValuePair<string, ValueContainer> item)
        {
            throw new System.NotImplementedException();
        }

        public bool ContainsKey(string key)
        {
            throw new System.NotImplementedException();
        }

        public void CopyTo(KeyValuePair<string, ValueContainer>[] array, int arrayIndex)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator<KeyValuePair<string, ValueContainer>> GetEnumerator()
        {
            return underlying.GetEnumerator();
        }

        public bool Remove(string key)
        {
            throw new System.NotImplementedException();
        }

        public bool Remove(KeyValuePair<string, ValueContainer> item)
        {
            throw new System.NotImplementedException();
        }

        public bool TryGetValue(string key, out ValueContainer value)
        {
            throw new System.NotImplementedException();
        }

        public void Watch(Action<object> listener)
        {
            this.listeners.Add(listener);
            listener.Invoke(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return underlying.GetEnumerator();
        }

        public void NotifyListeners()
        {
            foreach(var listener in listeners)
            {
                listener.Invoke(this);
            }
        }
    }
}